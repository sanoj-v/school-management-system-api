using DotNetCoreAPI.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DotNetCoreAPI.Repository
{
    public class UserRepository
    {
        public static UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            if (login.Username == "sanoj" && login.Password == "12345")
            {
                user = new UserModel { Id = "U123455", Name = "Sanoj Vishwakarma", Email = "sanoj.vishwakarma@gmail.com" };
            }else if (login.Username == "manoj" && login.Password == "12345")
            {
                user = new UserModel { Id = "U1211111", Name = "Manoj Vishwakarma", Email = "manoj.vishwakarma@gmail.com" };
            }
            return user;
        }
    }
}
