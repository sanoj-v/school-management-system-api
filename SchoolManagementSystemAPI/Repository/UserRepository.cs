using SchoolManagementSystemAPI.Models;
using System.Data;
using SchoolManagementSystemAPI.Classes;
using System.Data.SqlClient;
using System;

namespace SchoolManagementSystemAPI.Repository
{
    public class UserRepository
    {
        public static UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            if (login.Username == "sanoj" && login.Password == "12345")
            {
                user = new UserModel { Id = "U123455", Name = "Sanoj Vishwakarma", Email = "sanoj.vishwakarma@gmail.com" };
            }
            else if (login.Username == "manoj" && login.Password == "12345")
            {
                user = new UserModel { Id = "U1211111", Name = "Manoj Vishwakarma", Email = "manoj.vishwakarma@gmail.com" };
            }
            return user;
        }

        public DataTable GetUser()
        {
            DataSet ds = new DataSet();
            SqlDataReader reader;
            DataAccessLayer dal = new DataAccessLayer();
            reader = dal.ExecuteReader(ConnectionStrings.DefaultConnection, CommandType.StoredProcedure, "USP_GETCATEGORY", "ALL", null);
            DataTable _dt = new DataTable();
            _dt.Load(reader);
            return _dt;
        }

        public string InsertUser(UserModel _u)
        {
            int result = 0;
            DataAccessLayer dal = new DataAccessLayer();
            //result = dal.ExecuteNonQuery(ConnectionStrings.DefaultConnection, CommandType.StoredProcedure, "USP_GETCATEGORY", "ALL", null);
            if (result > 0) {
                return "Record Inserted Successfully";
            }
            throw new Exception("Something Went Wrong!");
        }
    }
}
