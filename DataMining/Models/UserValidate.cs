using DataMining.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataMining.Models
{
    public class UserValidate
    {
        //This method is used to check the user credentials
        public static bool Login(string username, string password)
        {
            using (MyDataEntities entity = new MyDataEntities())
            {
                return entity.APILogins.Any(x => x.username.Equals(username, StringComparison.OrdinalIgnoreCase) && x.password == password);
            }
        }

        //This method is used to return the User Details
        public static APILogin GetUserDetails(string username, string password)
        {
            MyDataEntities entity = new MyDataEntities();

            return entity.APILogins.FirstOrDefault(user =>
                user.username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && user.password == password);
        }
    }
}