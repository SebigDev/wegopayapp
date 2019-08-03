using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using userprofile.entities.DTOs;
using userprofile.entities.Models;
using userprofile.entities.RequestResponse;

namespace userprofile.core.Managers.UserManager
{
    public interface IUserManagerService
    {
        Task<LoginResponse> UserLogin(LoginModelDto modelDto);
        Task<RegistrationResponse> UserRegistration(RegisterUserModelDto modelDto);
        Task<bool> UserActivation(UserActivationDto activationDto);
        Task<bool> AdminAuthentication(string EmailAddress, string password);

        Task<bool> IsActivated(Expression<Func<UserModel, bool>> expression);

        Task<UserModel> CheckUserExists(Expression<Func<UserModel, bool>> expression);
    }
}
