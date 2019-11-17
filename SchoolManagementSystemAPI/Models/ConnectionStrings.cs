using SchoolManagementSystemAPI.Classes;
using System;

namespace SchoolManagementSystemAPI.Models
{
    public class ConnectionStrings
    {
        public static string DefaultConnection
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSetting["ConnectionStrings:DefaultConnection"]);
            }
        }
        public static string BackupConnection
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSetting["ConnectionStrings:BackupConnection"]);
            }
        }
    }
}
