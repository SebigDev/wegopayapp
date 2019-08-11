using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using userprofile.entities.DTOs;
using userprofile.entities.Models;
using userprofile.entities.RequestResponse;
using userprofile.persistence;
using wegopay.notification.Services.Process;

namespace userprofile.core.Managers.UserManager
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserProfileDbContext _userProfileDbContext;
        private readonly INotificationProcessor _notification;


        public UserManagerService(UserProfileDbContext userProfileDbContext, INotificationProcessor notification)
        {
            _userProfileDbContext = userProfileDbContext;
            _notification = notification;
        }

        public async Task<IEnumerable<UserModelDto>> RetrieveUsers()
        {
           
            var users =  await _userProfileDbContext.UserModel.ToListAsync();
            var dtoUsers = new List<UserModelDto>();
            dtoUsers.AddRange(users.OrderBy(x => x.DateCreated).Select(model => new UserModelDto
            {
                Id = model.Id,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                EmailAddress = model.EmailAddress,
                DateCreated = model.DateCreated,
                PhoneNo = model.PhoneNo,
                IsActivated = model.IsActivated,
                IsActive = model.IsActive,
                ReferredBy = model.ReferredBy
            }));
            return dtoUsers;
        }


        public async Task<UserModelDto> RetrieveUserByUserId(long Id)
        {
            var model = await _userProfileDbContext.UserModel.FirstOrDefaultAsync(x => x.Id == Id);
            var dtoUser = new UserModelDto();
            dtoUser = new UserModelDto
            {
                Id = model.Id,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                EmailAddress = model.EmailAddress,
                DateCreated = model.DateCreated,
                PhoneNo = model.PhoneNo,
                IsActivated = model.IsActivated,
                IsActive = model.IsActive,
                ReferredBy = model.ReferredBy
            };
            return dtoUser;
        }



       #region User Auth and Creation
    public async Task<bool> UserActivation(UserActivationDto activationDto)
        {
            using (var _activateTrans = _userProfileDbContext.Database.BeginTransaction())
            {
                var activate = await _userProfileDbContext.UserActivationModel.FirstOrDefaultAsync(a => a.ActivationCode == activationDto.ActivationCode &&
                                                                                                        a.UserModelId == activationDto.UserId);
                if (activate != null)
                {
                    //check if code has expired
                    var checkTime = activate.ExpiresAt >= (DateTime.UtcNow) ? true : false;
                    if (checkTime)
                    {
                        //delete the code from the database
                        _userProfileDbContext.Remove(activate);
                        await _userProfileDbContext.SaveChangesAsync();

                        //update user table with statuses
                        var user = await _userProfileDbContext.UserModel.FirstOrDefaultAsync(u => u.Id == activationDto.UserId);
                        if (user != null)
                        {
                            user.IsActivated = true;
                            user.IsActive = true;

                            _userProfileDbContext.Entry(user).State = EntityState.Modified;
                            await _userProfileDbContext.SaveChangesAsync();
                        }

                        //commit to database
                        _activateTrans.Commit();

                        return true;
                    }
                    return false;
                }

            }
            return false;
        }

        public async Task<LoginResponse> UserLogin(LoginModelDto modelDto)
        {
            LoginResponse response;
            var checkUser = await CheckUserExists(u => u.EmailAddress == modelDto.EmailAddress || u.PhoneNo == modelDto.PhoneNo);
            if (checkUser != null)
            {
                var verifyPass = VerifyPasswordHash(modelDto.Password, checkUser.PaswordHash, checkUser.PasswordSalt);
                if (verifyPass)
                {
                    var status = await IsActivated(x => (x.EmailAddress == modelDto.EmailAddress || x.PhoneNo == modelDto.PhoneNo) && x.IsActivated == true);
                    if (status)
                    {
                        var token = await GenerateToken(modelDto);

                        //if user has account and is activated
                        response = new LoginResponse
                        {
                            Token = token,
                            EmailAddress = modelDto.EmailAddress,
                            IsActived = status,
                            IsSuccess = true
                        };
                        return response;
                    }

                    //if user has account and is not activated
                    response = new LoginResponse
                    {
                        Token = null,
                        EmailAddress = modelDto.EmailAddress,
                        IsActived = status,
                        IsSuccess = false
                    };
                    return response;
                }
            }
            //if user does not have account
            return null;

        }

        public async Task<RegistrationResponse> UserRegistration(RegisterUserModelDto modelDto)
        {
            RegistrationResponse resp;
            if (modelDto == null)
                throw new ArgumentNullException(nameof(modelDto));

            var isEmailTaken = await CheckUserExists(e => e.EmailAddress.Equals(modelDto.EmailAddress));
            if (isEmailTaken != null)
            {
                // if email is already taken
                resp = new RegistrationResponse
                {
                    IsEmailTaken = true,
                    IsSucceeded = false,
                };
                return resp;
            }

            //transaction
            using (var _savieTransaction = _userProfileDbContext.Database.BeginTransaction())
            {
                //create password encrypt
                CreatePasswordEncrypt(modelDto.Pasword, out byte[] passwordHash, out byte[] passwordSalt);

                var user = new UserModel
                {
                    Firstname = modelDto.Firstname,
                    Lastname = modelDto.Lastname,
                    EmailAddress = modelDto.EmailAddress,
                    PhoneNo = modelDto.PhoneNo,
                    Password = modelDto.Pasword,
                    ReferredBy = modelDto.ReferredBy,
                    PaswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    DateCreated = DateTime.UtcNow,
                    IsActivated = false,
                    IsActive = false,
                };
                await _userProfileDbContext.AddAsync(user);
                await _userProfileDbContext.SaveChangesAsync();

                if (user.Id > 0)
                {
                    //if user registration succeeds and email not taken
                    var rnd = new Random();
                    var rnd1 = rnd.Next(10, 99);
                    var rnd2 = rnd.Next(100, 999);
                    var rnd3 = rnd.Next(1000, 9999);
                    var code = rnd3 + user.EmailAddress.Substring(3, 5).ToUpper() + rnd2 + rnd1;
                    var name = user.Firstname;

                    if (user.EmailAddress != null && user.PhoneNo != null)
                    {
                        var activationCode = new UserActivationModel
                        {
                            ActivationCode = code,
                            SentOn = DateTime.UtcNow,
                            ExpiresAt = DateTime.UtcNow.AddMinutes(5),//expires after 5 Minutes
                            ActivatedOn = null,
                            ComfirmedActivation = false,
                            UserModelId = user.Id,

                        };
                        //save the activation code

                        await _userProfileDbContext.UserActivationModel.AddAsync(activationCode);
                        await _userProfileDbContext.SaveChangesAsync();

                    }
                    _savieTransaction.Commit();

                    // await Task.Run(() => _notification.SendActivationCodeBySMS(name, code, user.PhoneNo));
                    await Task.Run(() => _notification.SendActivationCodeByMail(name, code, user.EmailAddress));

                    resp = new RegistrationResponse
                    {
                        IsSucceeded = true,
                        IsEmailTaken = false,
                    };

                    return resp;
                }

            }

            // if user registraion fails
            return null;
        }

        public async Task<bool> AdminAuthentication(string emailAddress, string password)
        {
            if (!(string.IsNullOrEmpty(emailAddress) && string.IsNullOrEmpty(password)))
            {
                var admin = await _userProfileDbContext.AdminUserModel
                    .AnyAsync(a => a.EmailAddress == emailAddress && a.Password == password);
                if (admin) return true;
                return false;

            }
            return false;
        }


        public async Task<bool> IsActivated(Expression<Func<UserModel, bool>> expression)
        {
            var isActivated = await _userProfileDbContext.UserModel.FirstOrDefaultAsync(expression);
            if (isActivated != null) return true;
            return false;
        }
    
        public async Task<UserModel> CheckUserExists(Expression<Func<UserModel, bool>> expression)
        {
            var isUserExist = await _userProfileDbContext.UserModel.FirstOrDefaultAsync(expression);
            if (isUserExist != null) return isUserExist;
            return null;
        }

    #endregion

       #region Helpers

    /// <summary>
    /// Generates a token
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<string> GenerateToken(LoginModelDto user)
    {
        //Generating Token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("Super Secret Key");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                   //new Claim(),
                   new Claim(ClaimTypes.Name, user.EmailAddress)
            }),
            Expires = DateTime.Now.AddDays(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return await Task.FromResult(tokenString);
    }

    //Verify Password
    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != passwordHash[i])
                    return false;
            }
            return true;
        }
    }


    /// <summary>
    /// Creates an encrypted password
    /// </summary>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <param name="passwordSalt"></param>
    private void CreatePasswordEncrypt(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    #endregion
    }
}

