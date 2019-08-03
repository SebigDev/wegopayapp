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

namespace userprofile.core.Managers.UserManager
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserProfileDbContext _userProfileDbContext;

        public UserManagerService(UserProfileDbContext userProfileDbContext)
        {
            _userProfileDbContext = userProfileDbContext;
        }

        public async Task<bool> IsActivated(Expression<Func<UserModel, bool>> expression)
        {
            var isActivated =  await _userProfileDbContext.UserModel.FirstOrDefaultAsync(expression);
            if (isActivated != null) return true;
            return false;
        }

        public async Task<UserModel> CheckUserExists(Expression<Func<UserModel, bool>> expression)
        {
            var isUserExist = await _userProfileDbContext.UserModel.FirstOrDefaultAsync(expression);
            if (isUserExist != null) return isUserExist;
            return null;
        }

        public async Task<bool> UserActivation(UserActivationDto activationDto)
        {
            var isUser =  await CheckUserExists(e => e.Id == activationDto.UserId);
            if (isUser == null) return false;

            var activate = new UserActivationModel
            {
                UserModelId = activationDto.UserId,
                ActivationCode = activationDto.ActivationCode,
                SentOn = DateTime.UtcNow,
                ComfirmedActivation = false,
                ActivatedOn = DateTime.UtcNow,
                
            };
            await _userProfileDbContext.AddAsync(activate);
            await _userProfileDbContext.SaveChangesAsync();

            if (activate.Id > 0) return true;
            return false;

        }

        public async Task<LoginResponse> UserLogin(LoginModelDto modelDto)
        {

            LoginResponse response;
            
            var checkUser = await CheckUserExists(u => u.EmailAddress == modelDto.EmailAddress || u.PhoneNo == modelDto.PhoneNo);
            if (checkUser != null)
            {
                var verifyPass = VerifyPasswordHash(modelDto.Password,checkUser.PaswordHash, checkUser.PasswordSalt);
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

            var isEmalTaken = await CheckUserExists(e => e.EmailAddress.Equals(modelDto.EmailAddress));
            if(isEmalTaken != null)
            {
                // if email is already taken
                resp = new RegistrationResponse
                {
                    IsEmailTaken = true,
                    IsSucceeded = false,
                };
                return resp;
            }

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
                resp = new RegistrationResponse
                {
                    IsSucceeded = true,
                    IsEmailTaken = false,
                };
                return resp;
            }
            // if user registraion fails
            return null;
        }

        public async Task<bool> AdminAuthentication(string emailAddress, string password)
        {
            if(!(string.IsNullOrEmpty(emailAddress) && string.IsNullOrEmpty(password)))
            {
                var admin = await _userProfileDbContext.AdminUserModel
                    .AnyAsync(a => a.EmailAddress == emailAddress && a.Password == password);
                if (admin) return true;
                return false;
               
            }
            return false;
        }

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
