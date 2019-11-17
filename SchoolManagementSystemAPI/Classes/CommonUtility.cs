using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystemAPI.Classes
{
    public class CommonUtility
    {
        public static string SaveFileToFolder(IFormFile myFile)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            var fileName = $"{guid}{Path.GetExtension(myFile.FileName)}";
            var folderName = Folder.Students;
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fullPath = Path.Combine(pathToSave, fileName);
            var dbPath = Path.Combine(folderName, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                myFile.CopyTo(stream);
            }
            return fileName;
        }
    }

    public class Folder
    {
        public static string Students
        {
            get
            {
                return Path.Combine("Documents", "Students");
            }
        }
        public static string Teachers
        {
            get
            {
                return Path.Combine("Documents", "Teachers");
            }
        }
    }
}
