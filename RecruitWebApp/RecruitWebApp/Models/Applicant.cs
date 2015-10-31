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
    public class Applicant
    {
        public int ApplicantId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [ForeignKey("Country")]
        public int CountryId { get; set; }
        [Required]
        public string EmploymentStatus { get; set; }
        [Required]
        public string AcademicQualification { get; set; }
        [Required]
        public string StudySpecialism { get; set; }
        [Required]
        public string CV { get; set; }
        [Required]
        public string PersonalImage { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNum { get; set; }
        public virtual Country Country { get; set; }


    }
}