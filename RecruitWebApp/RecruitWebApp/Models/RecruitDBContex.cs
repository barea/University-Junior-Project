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
    public class RecruitDBContex : DbContext
    {
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<JobReqest> JobReqests { get; set; }
        public DbSet<Country> Countries { get; set; }

        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}