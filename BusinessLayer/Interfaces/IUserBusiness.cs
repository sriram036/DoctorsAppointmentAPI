using ModelLayer.Models;
using System;

namespace BusinessLayer.Interfaces
{
    public interface IUserBusiness
    {
        UserModel AddUser(UserModel userModel);

        string LoginUser(LoginModel loginModel);

        string ForgotPassword(string Email);

        bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel);

        UserModel UpdateUser(string Email, string FirstName, string LastName, string Address, string DOB, string MobileNumber);

        bool DeleteUser(string Email);
    }
}