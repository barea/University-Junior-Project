using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecruitWebApp.Models;


namespace RecruitWebApp.Controllers
{
    public class JobReqestController : Controller
    {
        private RecruitDBContex db = new RecruitDBContex();

        //// POST: /JobReqest/Create

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Applicant")]
        public ActionResult Create(JobReqest jobreqest, int jobId)
        {

            //Check if user logged in
            if (ModelState.IsValid && Session["UID"] != null)
            {
                int appID = int.Parse(Session["UID"].ToString());
                int IsApplied = (from t in db.JobReqests
                                 where t.Jobnum == jobId && t.ApplicantId == appID
                                 select t.JobReqestId).Count();

                //check if user previously applied  for this job 
                if (IsApplied == 0)
                {
                    //user not applied --> create job request
                    jobreqest.Jobnum = jobId;
                    jobreqest.ApplicantId = int.Parse(Session["UID"].ToString());
                    db.JobReqests.Add(jobreqest);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Applicant", new { id = appID });
                }
                else
                {
                    //user applied --> display list of applied jobs
                    return View("JobApply");

                }

                }
            else
                return RedirectToAction("Login", "Account", null);

            }
            
    
    

        //return job by company
        [Authorize(Roles = "Company")]
        public ActionResult ComJob()
        {
            if (Session["companyID"] != null)
            {
                int comId = int.Parse(Session["companyID"].ToString());
                var job = from t in db.Jobs
                          where t.CompId == comId
                          select t;

                return View(job);
            }
            else
                return RedirectToAction("Login", "Account");
           
        }
        //view job Request 
        [Authorize(Roles = "Company")]
        public ActionResult ApplReq(string gender, string EmplStatus, int? Country, string Qualification,  int? minage, int? maxage, int Id = 0)
        {
            ViewBag.Country = new SelectList(db.Countries, "CountryId", "CountryName");
            int jobId = Id;
            var JobReqest = (from d in db.JobReqests
                             where d.Jobnum == jobId 
                             select d).Include(j => j.Applicant).Distinct();

            if (gender == "Male")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.Gender == "Male");
            }
            else if (gender == "Female")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.Gender == "Female");
            }
            if(Country != null )
            {
                JobReqest = JobReqest.Where(s => s.Applicant.CountryId == Country);
            }
            
            if (EmplStatus == "Employed")
           {
               JobReqest = JobReqest.Where(s => s.Applicant.EmploymentStatus == "Employed");
           }
            else if (EmplStatus == "Unemployed")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.EmploymentStatus == "Unemployed");
            }
            if (minage != null && maxage != null )
            {
                minage = -minage;
                maxage = -maxage;
                DateTime d = DateTime.Now;
                JobReqest = JobReqest.Where(s => s.Applicant.DateOfBirth.Year - d.Year <= minage && s.Applicant.DateOfBirth.Year - d.Year >= maxage);
            }
            if (Qualification == "Bachelor")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.AcademicQualification == "Bachelor");
            }
            else if (Qualification == "Master")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.AcademicQualification == "Master");
            }
            else if (Qualification == "PhD")
            {
                JobReqest = JobReqest.Where(s => s.Applicant.AcademicQualification == "PhD");
            }
           
               return View(JobReqest);
        }

        //Applicant details
        [Authorize(Roles = "Company")]
        public ActionResult ApplDetails(int Id = 0)
        {

            var applicant = (from t in db.Applicants
                             where t.ApplicantId == Id
                             select t).Include(s => s.Country);

            return View(applicant);
        }

        //Applicant cv 
        public ActionResult ApplCV(string filename)
        {
            string subpath = Server.MapPath("~/CV/");
            string path = subpath + filename;
            return File(path, "application/pdf");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}