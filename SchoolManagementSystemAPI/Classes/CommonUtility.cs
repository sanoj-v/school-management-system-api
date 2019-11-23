using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace SchoolManagementSystemAPI.Classes
{
    public class CommonUtility
    {
        public static string SaveFileToFolder(IFormFile myFile, string DirectoryName, string Id = null)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            var fileName = $"{guid}{Path.GetExtension(myFile.FileName)}";
            string folderName = $"{DirectoryName}";
            if (Id != null)
            {
                folderName = $"{DirectoryName}//{Id}";
            }
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fullPath = Path.Combine(pathToSave, fileName);
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
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
