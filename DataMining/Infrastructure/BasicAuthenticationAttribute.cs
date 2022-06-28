using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace DataMining.Infrastructure
{
    /// <summary>
    /// Used to Configure key aspects of Ver 2 API for Data Sharing
    /// </summary>
    /// <returns>API Functions for Basic Authentication</returns>
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        /// <summary>
        /// Used to Configure key aspects of Ver 2 API for Data Sharing
        /// </summary>
        /// <returns>API Functions for On Authorization, Authenticate</returns>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                var userName = usernamePasswordArray[0];
                var password = usernamePasswordArray[1];

                // Replace this with your own system of security / means of validating credentials

                bool varuser = OrgSecurity.Login(userName, password);
                if (varuser)
                {
                    // Initialization.   
                    char mChar = ',';
                    var role = OrgSecurity.GetRole(userName).Split(mChar);

                    var principal = new GenericPrincipal(new GenericIdentity(userName), role);
                    Thread.CurrentPrincipal = principal;
                    this.SignInUser(userName, true, actionContext, role);
                }
                else
                {
                    // Setting.    
                    HandleUnathorized(actionContext);
                }
            }
            else
            {
                HandleUnathorized(actionContext);
            }
        }

        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:'");
        }

        //#region Sign In method.    
        ///// <summary>  
        ///// Sign In User method.    
        ///// </summary>  
        ///// <param name="username">Username parameter.</param>  
        ///// <param name="isPersistent">Is persistent parameter.</param>  
        private void SignInUser(string username, bool isPersistent, HttpActionContext myRequest, string[] role)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                foreach (var mrole in role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, mrole));
                }
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = myRequest.Request.GetOwinContext();
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
        //#endregion
    }
}