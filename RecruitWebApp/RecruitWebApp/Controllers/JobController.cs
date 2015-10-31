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
    public class JobController : Controller
    {
        private RecruitDBContex db = new RecruitDBContex();

       // GET: /Job/Details/5

        public ActionResult Details(int id = 0)
        {
            Job job = db.Jobs.Find(id);
            string company = (from d in db.Companies
                              where d.CompId == job.CompId
                              select d.CompanyName).First();
            ViewBag.company = company;
            string About = (from d in db.Companies
                              where d.CompId == job.CompId
                              select d.About).First();
            ViewBag.About = About;
            int companyId = (from d in db.Companies
                             where d.CompId == job.CompId
                             select d.ID).First();
            ViewBag.ID = companyId;

            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        //
        // GET: /Job/Create
        [Authorize(Roles = "Company")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.comId = Session["companyID"];
            return View();
        }

        //
        // POST: /Job/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public ActionResult Create(Job job)
        {
            if (Session["companyID"] != null)
            {

                if (ModelState.IsValid)
                {

                    job.CompId = int.Parse(Session["companyID"].ToString());
                    db.Jobs.Add(job);
                    db.SaveChanges();
                    return RedirectToAction("ComJob", "JobReqest");
                }

                ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", job.CategoryId);
                ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", job.CountryId);
                return View(job);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        // search job by title & country
        public ActionResult Search(string jobtitle, int? Country)
        {

           
            ViewBag.Country = new SelectList(db.Countries, "CountryId", "CountryName");
            var job = (from t in db.Jobs
                       select t).Include(j => j.Category).Include(j => j.Company) ;
            if (!string.IsNullOrEmpty(jobtitle))
            {
               
                job = job.Where(s => s.JobTitle.StartsWith(jobtitle));
            }
            if (Country == null)
            {
                return View(job);
            }
            else
            {

                return View(job.Where(s => s.CountryId == Country));
            }

        }

        //Job Filter
        public ActionResult Filter(string jobtitle, int? categoryId, int? Country, bool? Full, bool? Part, bool? Contr, bool? Free, decimal? salrfrom, decimal? salrto, int? Experience, string date)
        {
            ViewBag.Country = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.categoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            var job = from d in db.Jobs
                      select d;
            //job title
            if (!string.IsNullOrEmpty(jobtitle))
            {
                job = job.Where(s => s.JobTitle.StartsWith(jobtitle));
            }
            //job category
            if (categoryId != null)
            {
                job = job.Where(s => s.Category.CategoryId == categoryId);
            }
            //Country
            if (Country != null)
            {
                job = job.Where(s => s.CountryId == Country);
            }
            //job type
            if (Full == true && Part == false && Contr == false && Free == false)
            {
                job = job.Where(s => s.JobType == "Full Time");
            }
            else if (Full == true && Part == true && Contr == false && Free == false)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Part Time");
            }
            else if (Full == true && Part == false && Contr == true && Free == false)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Contract");
            }
            else if (Full == true && Part == false && Contr == false && Free == true)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Freelance");
            }
            else if (Full == true && Part == true && Contr == true && Free == false)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Part Time" || s.JobType == "Contract");
            }
            else if (Full == true && Part == true && Contr == false && Free == true)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Part Time" || s.JobType == "Freelance");
            }
            else if (Full == true && Part == false && Contr == true && Free == true)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Contract" || s.JobType == "Freelance");
            }
            else if (Full == true && Part == true && Contr == true && Free == true)
            {
                job = job.Where(s => s.JobType == "Full Time" || s.JobType == "Part Time" || s.JobType == "Contract" || s.JobType == "Freelance");
            }
            else if (Full == false && Part == true && Contr == false && Free == false)
            {
                job = job.Where(s => s.JobType == "Part Time");
            }
            else if (Full == false && Part == true && Contr == true && Free == false)
            {
                job = job.Where(s => s.JobType == "Part Time" || s.JobType == "Contract");
            }
            else if (Full == false && Part == true && Contr == false && Free == true)
            {
                job = job.Where(s => s.JobType == "Part Time" || s.JobType == "Freelance");
            }
            else if (Full == false && Part == true && Contr == true && Free == true)
            {
                job = job.Where(s => s.JobType == "Part Time" || s.JobType == "Contract" || s.JobType == "Freelance");
            }
            else if (Full == false && Part == false && Contr == true && Free == false)
            {
                job = job.Where(s => s.JobType == "Contract");
            }
            else if (Full == false && Part == false && Contr == true && Free == true)
            {
                job = job.Where(s => s.JobType == "Contract" || s.JobType == "Freelance");
            }
            else if (Full == false && Part == false && Contr == false && Free == true)
            {
                job = job.Where(s => s.JobType == "Freelance");
            }
            //salary range

            if (salrfrom != null && salrto != null)
            {
                job = job.Where(s => s.Salary >= salrfrom && s.Salary <= salrto);
            }
            //Experience
            if (Experience != null)
            {
                job = job.Where(s => s.YearsOfExperience <= Experience);
            }
            //post date
            if (date != null)
            {

                DateTime today = DateTime.Now.Date;
                DateTime pdate;


                if (date == "Today")
                {
                    job = job.Where(s => s.PostDate == today);
                }
                else if (date == "Any")
                {
                    job = job.Where(s => s.PostDate <= today);
                }
                else
                {
                    double t = double.Parse(date);
                    pdate = today.AddDays(t);
                    job = job.Where(s => s.PostDate >= pdate && s.PostDate <= today);
                }

            }
            return View(job);
        }

        //
        // GET: /Job/Edit/5

        [Authorize(Roles = "Company")]
        public ActionResult Edit(int id = 0)
        {
            
                Job job = db.Jobs.Find(id);
                if (job.CompId == int.Parse(Session["companyID"].ToString()))
                {
                    if (job == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", job.CategoryId);
                    ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", job.CountryId);
                    return View(job);
                }
                else
                    return View("Access denied");
                    
        }

        //
        // POST: /Job/Edit/5
        [Authorize(Roles = "Company")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Job job)
        {
            
                 if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ComJob", "JobReqest");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", job.CategoryId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", job.CountryId);
            return View(job);
            
           
        }

        //
        // GET: /Job/Delete/5
        [Authorize(Roles = "Company")]
        public ActionResult Delete(int id = 0)
        {
            Job job = db.Jobs.Find(id);
             if (job.CompId == int.Parse(Session["companyID"].ToString())) 
            {
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
            }
             else
                 return View("Access denied");
        }

        //
        // POST: /Job/Delete/5
        [Authorize(Roles = "Company")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Job job = db.Jobs.Find(id);
            db.Jobs.Remove(job);
            db.SaveChanges();
            return RedirectToAction("ComJob", "JobReqest");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}