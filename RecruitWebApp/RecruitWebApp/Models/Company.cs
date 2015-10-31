using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace RecruitWebApp.Models
{
    public class Company
    {
        public int ID { get; set; }
        [Required]
        public int CompId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EstablishDate { get; set; }
        [Required]
        public string Logo { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string About { get; set; }
        public virtual Country Country { get; set; }
   
    }
}