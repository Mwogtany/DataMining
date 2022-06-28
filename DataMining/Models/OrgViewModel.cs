using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataMining.Models
{
    public class OrgViewModel
    {
        [Key]
        public string username { get; set; }
        public string active { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        [Display(Name = "User Role")]
        public string UserRole { get; set; }
        [Required]
        [Display(Name = "Org. KRA PIN")]
        public string OrgKRAPIN { get; set; }
        [DataType(DataType.MultilineText)]
        public string Purpose { get; set; }
        [Required]
        [Display(Name = "Data Requirement")]
        [DataType(DataType.MultilineText)]
        public string DataRequirements { get; set; }
        public Nullable<int> ReasonID { get; set; }
        [Required]
        [Display(Name = "Reason for Service")]
        public string ReasonDescription { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        [DataType(DataType.MultilineText)]
        public string PhysicalAddress { get; set; }

    }
    public class OrgViewModel2
    {
        [Key]
        public string username { get; set; }
        public string active { get; set; }
        [Required]
        public string Organization { get; set; }
        [Required]
        [Display(Name = "User Role")]
        public string UserRole { get; set; }
        [Required]
        [Display(Name = "Org. KRA PIN")]
        public string OrgKRAPIN { get; set; }
        [DataType(DataType.MultilineText)]
        public string Purpose { get; set; }
        [Required]
        [Display(Name = "Data Requirement")]
        [DataType(DataType.MultilineText)]
        public string DataRequirements { get; set; }
        public Nullable<int> ReasonID { get; set; }
        [Required]
        [Display(Name = "Reason for Service")]
        public string ReasonDescription { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Display(Name = "E-mail")]
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [DataType(DataType.MultilineText)]
        public string PhysicalAddress { get; set; }

        //Drop Down List
        public IList<SelectListItem> ReasonList { get; set; }

    }
}