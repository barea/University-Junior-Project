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
    public class CompanyController : Controller
    {
        private RecruitDBContex db = new RecruitDBContex();

        
        // GET: /Company/Details/5

        public ActionResult Details(int id = 0)
        {
            if(Session["CID"] != null)
            {
                int ComSession = int.Parse(Session["CID"].ToString());
                if (ComSession == id)
                {
                    Company company = db.Companies.Find(id);
                    if (company == null)
                    {
                        return HttpNotFound();
                    }
                    return View(company);
                }
                else
                    return View("Access denied");
               
            }
            else
                return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Company/Create

        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.ComName = Session["ComName"];
            ViewBag.ComID = Session["ComID"];
            ViewBag.ComPass = Session["ComPass"];
            return View();
        }

        //
        // POST: /Company/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company company, HttpPostedFileBase Logo)
        {
            if (Session["ComID"] != null)
            {
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/Logo/");
                    try
                    {
                        //Upload Logo
                        if (Logo != null)
                        {
                           Logo.SaveAs(path + company.Logo);

                        }
                    }
                    catch
                    {
                        // return RedirectToAction("Error", "Home", new { @ErrorMsg = "Error" });
                    }
                    db.Companies.Add(company);
                    db.SaveChanges();
                    string username = Session["ComName"].ToString();

                    //Account Confirmation
                    string confirmtoken = Session["Comtoken"].ToString();
                    WebSecurity.ConfirmAccount(username, confirmtoken);

                    //Add Company to Roles
                    Roles.AddUserToRole(username, "Company");

                    //Log in
                    WebSecurity.Login(username, company.Password);
                    Session["companyID"] = company.CompId;
                    Session["CID"] = company.ID;
                    return RedirectToAction("Details", "Company", new { id = company.ID });
                }

                ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", company.CountryId);
                return View(company);
            }
            else
                return RedirectToAction("Registeration", "Account", null);
        }

        //
        // GET: /Company/Edit/5
        [Authorize(Roles = "Company")]
        public ActionResult Edit(int id = 0)
        {
            if(Session["CID"] != null)
            {
                int ComSession = int.Parse(Session["CID"].ToString());
                if(ComSession == id)
                {
                    Company company = db.Companies.Find(id);
                    if (company == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", company.CountryId);
                    return View(company);
                }
                else
                    return View("Access denied");
               
            }
            else
                return RedirectToAction("Login", "Account");
        }

        //
        // POST: /Company/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Company company)
        {
            if (ModelState.IsValid)
            {
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Company", new { id = company.ID});
            }
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", company.CountryId);
            return View(company);
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}