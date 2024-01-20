using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using System;

namespace DoctorsAppointmentSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness userBusiness;

        public UserController(IUserBusiness userBusiness)
        {
            this.userBusiness = userBusiness;
        }

        [HttpPost]
        [Route("AddUser")]
        public IActionResult AddUser(UserModel userModel)
        {
            UserModel User = userBusiness.AddUser(userModel);
            if(userModel == null)
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Added", Data = "Connection Failed" });
            }
            else
            {
                return Ok(new ResponseModel<UserModel> { IsSuccess = true, Message = "User Added", Data = User });
            }
        }

        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUser(LoginModel loginModel)
        {
            string userModel = userBusiness.LoginUser(loginModel);

            if( userModel == null )
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Found", Data = "Email Not Matched" });
            }
            else
            {
                return Ok(new ResponseModel<string> { IsSuccess = true, Message = "User Found", Data = userModel });
            }
        }

        [HttpGet]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword(string Email)
        {
            string IsExist = userBusiness.ForgotPassword(Email);

            if(IsExist != null)
            {
                return Ok(new ResponseModel<string> { IsSuccess = true, Message = "User Exist", Data = IsExist });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Exist", Data = "Email not Found" });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            string Email = User.FindFirst("Email").Value;
            bool IsSuccess = userBusiness.ResetPassword(Email, resetPasswordModel);

            if (IsSuccess)
            {
                return Ok(new ResponseModel<string> { IsSuccess = true, Message = "User Found", Data = "Password Changed" });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Found", Data = "Password Not Changed" });
            }
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult UpdateUser(UserModel userModel)
        {
            string Email = User.FindFirst("Email").Value;
            UserModel user = userBusiness.UpdateUser(Email, userModel.FirstName, userModel.LastName, userModel.Address, userModel.DOB, userModel.MobileNumber);

            if(user != null)
            {
                return Ok(new ResponseModel<UserModel> { IsSuccess = true, Message = "User Updated", Data =  user });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Updated", Data = "Email Not Found" });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteUser")]
        public IActionResult DeleteUser()
        {
            string Email = User.FindFirst("Email").Value;
            bool IsDeleted = userBusiness.DeleteUser(Email);

            if (IsDeleted)
            {
                return Ok(new ResponseModel<string> { IsSuccess = true, Message = "User Deleted", Data = "Email Found" });
            }
            else
            {
                return BadRequest(new ResponseModel<string> { IsSuccess = false, Message = "User Not Deleted", Data = "Email Not Found" });
            }
        }
    }
}
