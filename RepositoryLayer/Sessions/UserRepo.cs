using Microsoft.Extensions.Configuration;
using ModelLayer.Models;
using System.Data;
using System;
using System.Data.SqlClient;
using RepositoryLayer.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace RepositoryLayer.Sessions
{
    public class UserRepo : IUserRepo
    {
        private readonly IConfiguration configuration;

        public UserRepo(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public UserModel AddUser(UserModel userModel)
        {
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                SqlCommand cmd = new SqlCommand("spAddUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FirstName", userModel.FirstName);
                cmd.Parameters.AddWithValue("@LastName", userModel.LastName);
                cmd.Parameters.AddWithValue("@Address", userModel.Address);
                cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(userModel.DOB));
                cmd.Parameters.AddWithValue("@Email", userModel.Email);
                cmd.Parameters.AddWithValue("@MobileNumber", long.Parse(userModel.MobileNumber));
                cmd.Parameters.AddWithValue("@Password", EncodePassword(userModel.Password));
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            return userModel;
        }

        public static string EncodePassword(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public string LoginUser(LoginModel loginModel)
        {
            int Id = 0;
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                SqlCommand cmd = new SqlCommand("spLoginUser", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email", loginModel.Email);
                cmd.Parameters.AddWithValue("@Password", EncodePassword(loginModel.Password));
                con.Open();

                UserModel userModel = new UserModel();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    Id = Convert.ToInt32(Reader["UserId"]);
                    userModel.FirstName = Reader["FirstName"].ToString();
                    userModel.LastName = Reader["LastName"].ToString();
                    userModel.Address = Reader["Address"].ToString();
                    userModel.DOB = Reader["DOB"].ToString();
                    userModel.Email = Reader["Email"].ToString();
                    userModel.MobileNumber = Reader["MobileNumber"].ToString();
                    userModel.Password = Reader["Password"].ToString();
                }
                con.Close();
                if(userModel.Email == loginModel.Email)
                {
                    return GenerateToken(loginModel.Email,Id);
                }
                else
                {
                    return null;
                }
            }
        }

        public string ForgotPassword(string Email)
        {
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                int Id = 0;
                SqlCommand cmd = new SqlCommand("spGetUserByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email", Email);
                con.Open();

                UserModel userModel = new UserModel();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    userModel.Email = Reader["Email"].ToString();
                    Id = Convert.ToInt32(Reader["UserId"]);
                }
                con.Close();
                if (userModel.Email != null)
                {
                    return GenerateToken(Email,Id);
                }
                else
                {
                    return null;
                }
            }
        }

        public string GenerateToken(string Email, int UserId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Email",Email),
                new Claim("UserId",UserId.ToString())
            };
            var token = new JwtSecurityToken(configuration["Jwt:Issue"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ResetPassword(string Email, ResetPasswordModel resetPasswordModel)
        {
            string EmailId = "";
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                SqlCommand cmd = new SqlCommand("spGetUserByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);

                con.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    EmailId = Reader["Email"].ToString();
                }
                if (EmailId == Email)
                {
                    SqlCommand cmdUpdate = new SqlCommand("spUpdatePassword", con);
                    cmdUpdate.CommandType = CommandType.StoredProcedure;
                    cmdUpdate.Parameters.AddWithValue("@Email", Email);
                    cmdUpdate.Parameters.AddWithValue("@Password", EncodePassword(resetPasswordModel.Password));
                    cmdUpdate.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmdUpdate.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public UserModel UpdateUser(string Email, string FirstName, string LastName, string Address, string DOB, string MobileNumber)
        {
            string EmailId = "";
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                SqlCommand cmd = new SqlCommand("spGetUserByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);

                con.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    EmailId = Reader["Email"].ToString();
                }
                con.Close();
                if (EmailId == Email)
                {
                    SqlCommand cmdUpdate = new SqlCommand("spUpdateUser", con);
                    cmdUpdate.CommandType = CommandType.StoredProcedure;
                    cmdUpdate.Parameters.AddWithValue("@FirstName", FirstName);
                    cmdUpdate.Parameters.AddWithValue("@LastName", LastName);
                    cmdUpdate.Parameters.AddWithValue("@Address", Address);
                    cmdUpdate.Parameters.AddWithValue("@DOB", Convert.ToDateTime(DOB));
                    cmdUpdate.Parameters.AddWithValue("@MobileNumber", long.Parse(MobileNumber));
                    cmdUpdate.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    cmdUpdate.Parameters.AddWithValue("@Email", Email);
                    con.Open();
                    cmdUpdate.ExecuteNonQuery();

                    UserModel userModel = new UserModel();
                    SqlCommand cmdGet = new SqlCommand("spGetUserByEmail", con);
                    cmdGet.CommandType = CommandType.StoredProcedure;
                    cmdGet.Parameters.AddWithValue("@Email", Email);

                    SqlDataReader ReaderData = cmd.ExecuteReader();
                    while (ReaderData.Read())
                    {
                        userModel.FirstName = ReaderData["FirstName"].ToString();
                        userModel.LastName = ReaderData["LastName"].ToString();
                        userModel.Address = ReaderData["Address"].ToString();
                        userModel.DOB = ReaderData["DOB"].ToString();
                        userModel.MobileNumber = ReaderData["MobileNumber"].ToString();
                        userModel.Email = ReaderData["Email"].ToString();
                        userModel.Password = ReaderData["Password"].ToString();
                    }
                    return userModel;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool DeleteUser(string Email)
        {
            string EmailId = "";
            using (SqlConnection con = new SqlConnection(configuration["ConnectionStrings:DoctorsAppointmentConnection"]))
            {
                SqlCommand cmd = new SqlCommand("spGetUserByEmail", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", Email);

                con.Open();
                SqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    EmailId = Reader["Email"].ToString();
                }
                if (EmailId == Email)
                {
                    SqlCommand cmdUpdate = new SqlCommand("spDeleteUser", con);
                    cmdUpdate.CommandType = CommandType.StoredProcedure;
                    cmdUpdate.Parameters.AddWithValue("@Email", Email);
                    cmdUpdate.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
