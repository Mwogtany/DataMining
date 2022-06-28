using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DataMining.Models
{
    // Models returned by AccountController actions.
    // Models returned by AccountController actions.
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get;
            set;
        }
    }
    public class LoginViewModel2
    {
        [Required]
        [Display(Name = "Username")]
        public string Username
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get;
            set;
        }
        public string Role
        {
            get;
            set;
        }
        public string Active
        {
            get;
            set;
        }
    }
    public class RegViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string username
        {
            get;
            set;
        }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password
        {
            get;
            set;
        }
        [Required]
        [Display(Name = "E-mail")]
        public string email { get; set; }
        [Required]
        [Display(Name = "Mobile")]
        public string mobile { get; set; }
        [Required]
        [Display(Name = "Question 1")]
        public Nullable<int> Q1 { get; set; }
        public string Q1Ans { get; set; }
        [Display(Name = "Question 2")]
        public Nullable<int> Q2 { get; set; }
        public string Q2Ans { get; set; }
        public Nullable<int> LoginAttempts { get; set; }

        public IList<SelectListItem> QtnList { get; set; }
    }
    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }

    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
