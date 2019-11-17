using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystemAPI.Models
{
    public class StudentModel
    {
        public string Model { get; set; }
        public IFormFile File { get; set; }
    }
}
