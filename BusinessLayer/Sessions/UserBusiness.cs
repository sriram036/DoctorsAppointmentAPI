using BusinessLayer.Interfaces;
using ModelLayer.Models;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Sessions
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepo userRepo;

        public UserBusiness(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public UserModel AddUser(UserModel userModel)
        {
            return userRepo.AddUser(userModel);
        }

        public string LoginUser(LoginModel loginModel)
        {
            return userRepo.LoginUser(loginModel);
        }

        public string ForgotPassword(string Email)
        {
            return userRepo.ForgotPassword(Email);
        }

        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            return userRepo.ResetPassword(Email, resetPasswordModel);
        }

        public UserModel UpdateUser(string Email, string FirstName, string LastName, string Address, string DOB, string MobileNumber)
        {
            return userRepo.UpdateUser(Email, FirstName, LastName, Address, DOB, MobileNumber);
        }

        public bool DeleteUser(string Email)
        {
            return userRepo.DeleteUser(Email);
        }
    }
}
