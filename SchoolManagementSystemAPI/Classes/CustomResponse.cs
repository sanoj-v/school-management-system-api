using DotNetCoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreAPI.Classes
{
    public class CustomResponse
    {
        public static object OkObject(object d)
        {
            //throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Please check username or password");
            //return Ok("string");
            //return new ObjectResult();
            return new { data = d, message = "Ok", status = 200 };
        }
    }
}
