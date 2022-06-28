using DataMining.DataAccess;
using DataMining.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataMining.Controllers
{
    [Authorize]
    public class OrgController : Controller
    {
        // GET: Org
        MyDataEntities Db = new MyDataEntities();
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            var muser = (string)Session["user"];
            var myapps = (from p in Db.APILogins
                          join x in Db.APIUsages on p.username equals x.username
                          join w in Db.STATIC_DATA_USE_REASON on x.ReasonID equals w.id
                          where p.username == muser
                          select new OrgViewModel2()
                          {
                              username = p.username,
                              active = p.active,
                              Organization = p.Organization,
                              UserRole = p.UserRole,
                              OrgKRAPIN = p.OrgKRAPIN,
                              Purpose = x.Purpose,
                              DataRequirements = x.DataRequirements,
                              ReasonID = x.ReasonID,
                              ReasonDescription = w.Description,
                              Mobile = p.Mobile
                          });

            var myapps2 = (from p in Db.proc_GetDailySummariesPerUser(muser)
                           select p).DefaultIfEmpty().ToList();

            ViewBag.mysummarylist = myapps2;

            return View(myapps);
        }

        //Get: View Services
        public ActionResult GetServices()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            ViewBag.Title = "Identity Services Page";

            var mservice = (from p in Db.STATIC_DATA_USE_REASON
                            where p.Active == "1"
                            select p).DefaultIfEmpty();

            return View(mservice);
        }

        //Get: View Services
        public ActionResult Apply()
        {

            ViewBag.Title = "Apply for Identity Service Product";
            var muser = (string)Session["user"];
            ViewBag.UserName = muser;
            var myapps = new OrgViewModel2();
            myapps.username = muser;
            List<STATIC_DATA_USE_REASON> dbreason = Db.STATIC_DATA_USE_REASON.Where(x => x.Active == "1").ToList();

            List<SelectListItem> mreason = new List<SelectListItem>();

            dbreason.ForEach(x =>
            {
                mreason.Add(new SelectListItem { Text = x.Description, Value = x.id.ToString() });
            });

            myapps.ReasonList = mreason;

            return View(myapps);
        }
        [HttpPost]
        public ActionResult Apply(OrgViewModel2 mymodel)
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            var mUser = (string)Session["user"];

            var aplog = Db.APILogins.Where(s => s.username == mUser).FirstOrDefault();
            if(aplog == null)
            {
                Db.APILogins.Add(new APILogin()
                {
                    username = mUser,
                    active = "0",
                    Organization = mymodel.Organization,
                    OrgKRAPIN = mymodel.OrgKRAPIN,
                    UserRole = "0",
                    createdon = DateTime.Now,
                    Mobile = mymodel.Mobile,
                    Email = mymodel.Email,
                    PhysicalAddress = mymodel.PhysicalAddress
                });
            }
            else
            {
                aplog.OrgKRAPIN = mymodel.OrgKRAPIN;
                aplog.Organization = mymodel.Organization;
                aplog.Mobile = mymodel.Mobile;
                aplog.Email = mymodel.Email;
                aplog.PhysicalAddress = mymodel.PhysicalAddress;
            }

            var appusage = Db.APIUsages.Where(p => p.username == mUser).FirstOrDefault();
            if (appusage == null)
            {
                Db.APIUsages.Add(new APIUsage()
                {
                    username = mUser,
                    ReasonID = mymodel.ReasonID,
                    DataRequirements = mymodel.DataRequirements,
                    Purpose = mymodel.Purpose
                });
            }
            else
            {
                appusage.ReasonID = mymodel.ReasonID;
                appusage.DataRequirements = mymodel.DataRequirements;
                appusage.Purpose = mymodel.Purpose;
            }
            try
            {
                // Your code...
                // Could also be before try if you know the exception occurs in SaveChanges
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            //Return with errors incase saving did not succeed
            return View();
        }

        public PartialViewResult PartialSearches()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }

            return PartialView();
        }
        public PartialViewResult PartialSearch()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }

            return PartialView();
        }

        public ActionResult Edit(string id)
        {

            ViewBag.Title = "Edit for Identity Service Product";
            var muser = (string)Session["user"];
            ViewBag.UserName = muser;
            var myapps = new OrgViewModel2();
            myapps.username = muser;
            List<STATIC_DATA_USE_REASON> dbreason = Db.STATIC_DATA_USE_REASON.Where(x => x.Active == "1").ToList();

            List<SelectListItem> mreason = new List<SelectListItem>();

            dbreason.ForEach(x =>
            {
                mreason.Add(new SelectListItem { Text = x.Description, Value = x.id.ToString() });
            });

            var mapprec = Db.vw_Applications.Where(p => p.username == muser).SingleOrDefault();
            if(mapprec != null)
            {
                myapps.Organization = mapprec.Organization;
                myapps.OrgKRAPIN = mapprec.OrgKRAPIN;
                myapps.Purpose = mapprec.Purpose;
                myapps.ReasonDescription = mapprec.Description;
                myapps.ReasonID = mapprec.ReasonID;
                myapps.UserRole = mapprec.UserRole;
                myapps.Mobile = mapprec.Mobile;
                myapps.Email = mapprec.Email;
                myapps.PhysicalAddress = mapprec.PhysicalAddress;
                myapps.DataRequirements = mapprec.DataRequirements;
                myapps.active = mapprec.active;
            }
            myapps.ReasonList = mreason;

            return View(myapps);
        }
    }
}