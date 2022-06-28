using DataMining.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataMining.Models
{
    public class UserMasterRepository : IDisposable
    {
        // NEMISEntities it is your context class
        MyDataEntities context = new MyDataEntities();
        //This method is used to check and validate the user credentials
        public APILogin ValidateUser(string username, string password)
        {
            return context.APILogins.FirstOrDefault(user =>
            user.username.Equals(username, StringComparison.OrdinalIgnoreCase)
            && user.password == password);
        }
        public void Dispose()
        {
            context.Dispose();
        }
    }
}