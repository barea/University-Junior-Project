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
    public class JobReqest
    {
        public int JobReqestId { get; set; }

        [Required]
        public int Jobnum { get; set; }

        [Required]
        [ForeignKey("Applicant")]
        public int ApplicantId { get; set; }
        public virtual Applicant Applicant { get; set; }
        public virtual Job Job { get; set; }
    }
}