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
    public class Job
    {
        public int JobId { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [Required]
        public int CompId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime PostDate { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public string JobType { get; set; }
        [Required]
        public int YearsOfExperience { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public virtual Category Category { get; set; }
        public virtual Company Company { get; set; }
        public virtual Country Country { get; set; }
    }
}