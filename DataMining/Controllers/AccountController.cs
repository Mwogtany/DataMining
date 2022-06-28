using DataMining.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web;
using DataMining.DataAccess;

//-----------------------------------------------------------------------    
// <copyright file="AccountController.cs" company="pluscodesafrica.org">  
//   Copyright (c) Allow to distribute this code.    
// </copyright>  
// <author>Barnabas K. Sang</author>  
//-----------------------------------------------------------------------  
namespace DataMining.Controllers
{
    /// <summary>  
    /// Account controller class.    
    /// </summary> 
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        MyDataEntities Db = new MyDataEntities(); 
        #region Default Constructor    
        /// <summary>  
        /// Initializes a new instance of the <see cref="AccountController" /> class.    
        /// </summary>  
        public AccountController()
        {
        }
        #endregion
        #region Login methods    
        /// <summary>  
        /// GET: /Account/Login    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter</param>  
        /// <returns>Return login view</returns>  
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.    
                if (this.Request.IsAuthenticated)
                {
                    // Info.    
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // Info.    
            return this.View();
        }
        /// <summary>  
        /// POST: /Account/Login    
        /// </summary>  
        /// <param name="model">Model parameter</param>  
        /// <param name="returnUrl">Return URL parameter</param>  
        /// <returns>Return login view</returns>  
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View();

            Session["mPass"] = model.Password;
            var mpass = encryptpass(model.Password);
            //var mpass = model.Password;
            var varuser = (from p in Db.Logins
                           join x in Db.APILogins on p.username equals x.username
                           where p.username == model.Username && p.password == mpass.ToString()
                           select new LoginViewModel2()
                           {
                               Username = p.username,
                               Role = x.UserRole,
                               Active = x.active
                           }).FirstOrDefault();

            if (varuser != null)
            {
                // Initialization.    
                var logindetails = varuser;
                // Login In.
                Session["user"] = logindetails.Username;
                var mrole = varuser.Role;
                Session["myroles"] = mrole;
                char mChar = ',';
                var roles = mrole.Split(mChar);
                this.SignInUser((string)Session["user"], false, roles);

                string madmrole = "Admin";

                if (Array.IndexOf(roles, madmrole) >= 0)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Info.    
                    //return this.RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Org");
                }
            }
            else
            {
                // Setting.    
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            

            return this.View(model);
        }

        public string encryptpass(string password)
        {
            string msg = "";
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            msg = Convert.ToBase64String(encode);
            return msg;
        }
        public string Decrypt(string clearText)
        {
            string EncryptionKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        /// <summary>  
        /// POST: /Account/LogOff    
        /// </summary>  
        /// <returns>Return log off action</returns>  
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.    
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register method.    
        /// <summary>  
        /// POST: /Account/Register    
        /// </summary>  
        /// <returns>Return Register action for new Users</returns> 
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            MyDataEntities Db = new MyDataEntities();
            RegViewModel ViewModel = new RegViewModel();
            List<SelectListItem> QtnList = new List<SelectListItem>();

            List<STATIC_REGISTER_QUESTION> myQtns = Db.STATIC_REGISTER_QUESTION.ToList();
            myQtns.ForEach(x =>
            {
                QtnList.Add(new SelectListItem { Text = x.Description, Value = x.id.ToString() });
            });
            ViewModel.QtnList = QtnList;

            return this.View(ViewModel);
        }

        /// <summary>  
        /// POST: /Account/Register    
        /// </summary>  
        /// <returns>Return Register action</returns>  
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegViewModel model)
        {
            try
            {
                // Verification.    
                if (ModelState.IsValid)
                {
                    MyDataEntities Db = new MyDataEntities();
                    // Initialization.    
                    var newpass = encryptpass(model.password.ToString());
                    // Verification.    
                    Db.Logins.Add(new Login()
                    {
                        username = model.username,
                        password = newpass,
                        email = model.email,
                        mobile = model.mobile,
                        Q1 = model.Q1,
                        Q1Ans = model.Q1Ans,
                        Q2 = model.Q2,
                        Q2Ans = model.Q2Ans,

                    });

                    Db.UserRoleAssignments.Add(new UserRoleAssignment()
                    {
                        UserID = model.username,
                        RoleID = 0,
                        ScopeID = 0,
                        SiteCode = "0",
                        CreatedBy = model.username,
                        CreatedOn = DateTime.Now
                    });
                    Db.SaveChanges();

                    Session["user"] = model.username;
                    string[] role = { "Guest" };

                    this.SignInUser((string)Session["user"], false, role);
                    // Info.    
                    return this.RedirectToAction("Index", "Org");

                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // If we got this far, something failed, redisplay form    
            return this.View(model);
        }
        #endregion

        #region Helpers    
        #region Sign In method.    
        /// <summary>  
        /// Sign In User method.    
        /// </summary>  
        /// <param name="username">Username parameter.</param>  
        /// <param name="isPersistent">Is persistent parameter.</param>  
        private void SignInUser(string username, bool isPersistent, string[] roles)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                foreach(var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                //var claimIdenties = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, DefaultAuthenticationTypes.ApplicationCookie,
                //ClaimTypes.Name, ClaimTypes.Role);
                //claimIdenties.AddClaim(new Claim(ClaimTypes.Name, username));
                
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }

        public void SignInUserAll(string username, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }
        #endregion

        public string Encryptor(string strText, string EncrKey)
        {
            byte[] byKey = { };
            byte[] IV =
              {
                18,52,86,120,144,171,205,239
            };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(strText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string DecryptFromBase64String(string stringToDecrypt, string EncryptionKey)
        {
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            byte[] byKey = { };
            byte[] IV =
            {
                18,52,86,120,144,171,205,239
            };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(stringToDecrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        #region Redirect to local method.    
        /// <summary>  
        /// Redirect to local method.    
        /// </summary>  
        /// <param name="returnUrl">Return URL parameter.</param>  
        /// <returns>Return redirection action</returns>  
        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.    
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.    
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Index", "Home");
        }
        #endregion
        #endregion
    }
}