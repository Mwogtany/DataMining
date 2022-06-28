using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataMining.Models
{
    public partial class IPRSViewModel
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorOcurred { get; set; }
        public string ID_Number { get; set; }
        public string Surname { get; set; }
        public string First_Name { get; set; }
        public string Other_Name { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        public string Citizenship { get; set; }
        public string Clan { get; set; }
        public string Ethnic_Group { get; set; }
        public string Family { get; set; }
        public string Serial_Number { get; set; }
        public string Place_of_Birth { get; set; }
        public string Place_of_Live { get; set; }
    }

    [Serializable]
    public partial class IPRSPersonViewModel
    {
        [Key]
        public string ID_Number { get; set; }
        public string Surname { get; set; }
        public string First_Name { get; set; }
        public string Other_Name { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> Date_of_Birth { get; set; }
        public string Citizenship { get; set; }
        public string Clan { get; set; }
        public string Ethnic_Group { get; set; }
        public string Family { get; set; }
        public string Serial_Number { get; set; }
        public string Place_of_Birth { get; set; }
        public string Place_of_Live { get; set; }
    }

    public partial class Person_Pin
    {
        public string ID_Number { get; set; }
        public string KRAPIN { get; set; }
    }
}