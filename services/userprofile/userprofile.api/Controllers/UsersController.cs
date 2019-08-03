﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userprofile.core.Managers.UserManager;
using userprofile.entities.DTOs;
using userprofile.entities.RequestResponse;

namespace userprofile.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagerService _userManagerService;
        public UsersController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AdminAuthentication(string emailAddress, string password)
        {
            try
            {
                var adminAuth = await _userManagerService.AdminAuthentication(emailAddress, password);
                if (adminAuth) return Ok(adminAuth);
                return Ok(adminAuth);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [Produces(typeof(ResponseDataObject<RegistrationResponse>))]
        public async Task<IActionResult> UserRegistration(RegisterUserModelDto model)
        {
            try
            {
                var registerResult = await _userManagerService.UserRegistration(model);
                if(registerResult.IsEmailTaken)
                {
                    var resp = new ResponseDataObject<RegistrationResponse>
                    {
                        Data = registerResult,
                        Message = "Email Address already taken, Please Use another or login",
                        Status = true,
                       
                    };
                    return Ok(resp);
                }
                if (registerResult.IsSucceeded)
                {
                    var resp = new ResponseDataObject<RegistrationResponse>
                    {
                        Data = registerResult,
                        Message = "User registration was successful",
                        Status = true,

                    };
                    return Ok(resp);
                }
                if (registerResult == null)
                {
                    var resp = new ResponseDataObject<RegistrationResponse>
                    {
                        Data = null,
                        Message = "User registration failed, Please contact Admin",
                        Status = true,

                    };
                    return Ok(resp);
                }
                return BadRequest("User Registration Failed");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [Produces(typeof(ResponseDataObject<LoginResponse>))]
        public async Task<IActionResult> UserLogin(LoginModelDto model)
        {
            try
            {
                var loginResult = await _userManagerService.UserLogin(model);

                if (loginResult == null)
                {
                    var res = new ResponseDataObject<LoginResponse>
                    {
                        Data = null,
                        Message = "Authentication failed!",
                        Status = false,
                    };
                    return Ok(res);
                }

                if (loginResult.IsActived)
                {
                    var res = new ResponseDataObject<LoginResponse>
                    {
                        Data = loginResult,
                        Message = "Login was successful",
                        Status = true,
                    };
                    return Ok(res);
                }
                if (!loginResult.IsSuccess)
                {
                    var res = new ResponseDataObject<LoginResponse>
                    {
                        Data = loginResult,
                        Message = "Your Account is not activated yet!",
                        Status = true,
                    };
                    return Ok(res);
                }
             
                return BadRequest("User login failed!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [Produces(typeof(bool))]
        public async Task<IActionResult> UserActivation(UserActivationDto model)
        {
            try
            {
                var activationResult = await _userManagerService.UserActivation(model);
                if (activationResult)
                {
                    return Ok($"User with {model.UserId} activated successfully");
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}