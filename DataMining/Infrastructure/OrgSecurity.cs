using DataMining.DataAccess;
using DataMining.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DataMining.Infrastructure
{
    /// <summary>
    /// Used to authenticate a user of API, NB:- Users are Organizational in Nature Allowed
    /// </summary>
    /// <returns>API Functions User Login</returns>
    public class OrgSecurity
    {
        /// <summary>
        /// Used to Encrypt Text
        /// </summary>
        /// <returns>string</returns>
        public static string encryptpass(string password)
        {
            string msg = "";
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            msg = Convert.ToBase64String(encode);
            return msg;
        }

        /// <summary>
        /// Used to login a user of API
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool Login(string uname, string pass)
        {
            using (MyDataEntities entity = new MyDataEntities())
            {
                //return entity.Logins.Any(x => x.username.Equals(uname, StringComparison.OrdinalIgnoreCase) && x.password == pass);

                var mpass = encryptpass(pass);
                //var mpass = model.Password;
                var varuser = (from p in entity.Logins
                               join x in entity.APILogins on p.username equals x.username
                               where p.username == uname && p.password == mpass.ToString()
                               select new LoginViewModel2()
                               {
                                   Username = p.username,
                                   Role = x.UserRole,
                                   Active = x.active
                               }).FirstOrDefault();

                if (varuser != null)
                {
                    // Initialization.    
                    var mActive = varuser.Active;
                    if (mActive == "1")
                    {
                        return true;
                    }
                    else
                    {
                        // Setting.    
                        return false;
                    }
                }
                else
                {
                    // Setting.    
                    return false;
                }
            }
            return false;
        }

        public static string GetRole(string uname)
        {
            using (MyDataEntities entity = new MyDataEntities())
            {
                var mRole = entity.APILogins.Where(p => p.username == uname).SingleOrDefault();
                if(mRole != null)
                {
                    return mRole.UserRole.ToString();
                }
                else
                {
                    return "Guest";
                }
            }
        }
    }
}