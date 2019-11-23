using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchoolManagementSystemAPI.Classes;
using SchoolManagementSystemAPI.Models;
using SchoolManagementSystemAPI.Repository;
using System;
using System.Linq;
using System.Net;

namespace SchoolManagementSystemAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        private string UserId;
        private string UserRole;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;

        public ValuesController(IConfiguration config, IHttpContextAccessor httpContextAccessor, IHostingEnvironment environment)
        {
            _configuration = config;
            _httpContextAccessor = httpContextAccessor;
            UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            _hostingEnvironment = environment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                UserRepository _user = new UserRepository();
                return Ok(new { status = 200, data = _user.GetUser(), message = "" });
            }
            catch (Exception ex)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public object ResponseType(object d)
        {
            return new { status = 200, data = d, message = "" };
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserModel _u)
        {
            try
            {
                UserRepository _user = new UserRepository();
                return Ok(_user.InsertUser(_u));
            }
            catch (Exception ex)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost, DisableRequestSizeLimit]
        public IActionResult PostFormData()
        {
            try
            {
                var myData = Request.Form["Model"];
                string fileId = string.Empty;
                UserModel myDetails = JsonConvert.DeserializeObject<UserModel>(myData);
                if (Request.Form.Files.Count > 0)
                {
                    fileId = CommonUtility.SaveFileToFolder(Request.Form.Files[0], Folder.Students, UserId);
                }
                myDetails.CreatedBy = UserId;

                UserRepository _user = new UserRepository();
                return Ok(myDetails);
            }
            catch (Exception ex)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok("");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
