using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecruitWebApp.Models;
using System.Transactions;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using RecruitWebApp.Filters;

namespace RecruitWebApp.Controllers
{
    public class ApplicantController : Controller
    {
        private RecruitDBContex db = new RecruitDBContex();

        //List Of Applied Job
        [Authorize(Roles = "Applicant")]
        public ActionResult appliedJob(int id = 0)
        {
            if (Session["UID"] != null)
            {
                int ApplSession = int.Parse(Session["UID"].ToString());
                if (ApplSession == id)
                {
                    var g = (from t in db.Jobs
                             join c in db.JobReqests
                             on t.JobId equals c.Jobnum
                             where c.ApplicantId == id
                             select t).Include(s => s.Company).ToList().Distinct();
                    return PartialView(g);
                }
                else
                    return View("Access denied");
                
            }
            else
                return RedirectToAction("Login", "Account");
           
        }
        //View CV
        public ActionResult ApplCV(string filename, int id = 0)
        {
            if(Session["UID"] != null)
            {
                int ApplSession = int.Parse(Session["UID"].ToString());
                if (ApplSession == id)
                {
                    string subpath = Server.MapPath("~/CV/");
                    string path = subpath + filename;
                    return File(path, "application/pdf");
                }
                else
                    return View("Access denied");
            }
            else
                return RedirectToAction("Login", "Account");
             
        }
       
        //
        // GET: /Applicant/Details/5

        public ActionResult Details(int id = 0)
        {
            if (Session["UID"] != null)
            {
                int ApplSession = int.Parse(Session["UID"].ToString());
                if (ApplSession == id)
                {
                    Applicant applicant = db.Applicants.Find(id);
                    if (applicant == null)
                    {
                        return HttpNotFound();
                    }
                    return View(applicant);
                }
                else
                    return View("Access denied");
                     
            }
            else
                return RedirectToAction("Login", "Account");
           
        }

        //
        // GET: /Applicant/Create

        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.AppName = Session["AppName"];
            ViewBag.AppID = Session["AppID"];
            ViewBag.AppPass = Session["AppPass"];
            return View();
        }

        //
        // POST: /Applicant/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Applicant applicant, HttpPostedFileBase CV, HttpPostedFileBase PersonalImage, string PhoneNum)
        {
            if (Session["AppID"] != null)
            {
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/CV/");
                    string path2 = Server.MapPath("~/AppImg/");

                    try
                    {
                        //Upload CV 
                        if (CV != null)
                        {
                            CV.SaveAs(path + applicant.CV);
                        }

                        //Upload Image
                        if (PersonalImage != null)
                        {
                            PersonalImage.SaveAs(path2 + applicant.PersonalImage);
                        }
                    }

                    catch
                    {
                        // return RedirectToAction("Error", "Home", new { @ErrorMsg = "Error" });
                        // return View("Error");
                    }
                    applicant.PhoneNum = PhoneNum;
                    db.Applicants.Add(applicant);
                    db.SaveChanges();

                    //Account Confirmation
                    string ConfirmToken = Session["Appltoken"].ToString();
                    WebSecurity.ConfirmAccount(applicant.UserName, ConfirmToken);

                    //Add Aplicant to Roles
                    Roles.AddUserToRole(applicant.UserName, "Applicant");

                    //Log In
                    WebSecurity.Login(applicant.UserName, applicant.Password);
                    Session["UID"] = applicant.ApplicantId;

                    return RedirectToAction("Details", "Applicant", new { id = applicant.ApplicantId });
                }

                ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", applicant.CountryId);
                return View(applicant);
            }
            else
                return RedirectToAction("Registeration", "Account", null);
           
        }

        //
        // GET: /Applicant/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Session["UID"] != null)
            {
                int ApplSession = int.Parse(Session["UID"].ToString());
                if (ApplSession == id)
                {
                    Applicant applicant = db.Applicants.Find(id);
                    ViewBag.AppName = Session["AppName"];
                    ViewBag.AppID = Session["AppID"];
                    ViewBag.AppPass = Session["AppPass"];
                    if (applicant == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", applicant.CountryId);
                    return View(applicant);
                }
                else
                    return View("Access denied");
            }
            else
                return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Applicant/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Applicant applicant, string PhoneNum)
        {
            if (ModelState.IsValid)
            {
                applicant.PhoneNum = PhoneNum;
                db.Entry(applicant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Applicant", new { id = applicant.ApplicantId });
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", applicant.CountryId);
            return View(applicant);
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}