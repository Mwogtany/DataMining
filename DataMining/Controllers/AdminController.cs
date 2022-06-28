using DataMining.DataAccess;
using DataMining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataMining.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        MyDataEntities Db = new MyDataEntities();
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            var muser = (string)Session["user"];
            var myapps = (from p in Db.vw_Applications
                          where p.username != muser
                          select p).DefaultIfEmpty().ToList();

            ViewBag.myuserlist = myapps;

            var myapps2 = (from p in Db.proc_GetDailySummariesForHomepage()
                          select p).DefaultIfEmpty().ToList();

            ViewBag.mysummarylist = myapps2;


            var myapps3 = (from p in Db.proc_GetSummaryResources()
                           select p).DefaultIfEmpty().ToList();

            ViewBag.myresourcelist = myapps3;
            return View();
        }

        public PartialViewResult PartialUserLists()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            ViewBag.Title = "Identity Applicants Page";

            var muser = (string)Session["user"];

            var myapps = (from p in Db.vw_Applications
                          where p.username != muser
                          select p).DefaultIfEmpty().ToList();
            return PartialView(myapps);
        }

        public PartialViewResult PartialSearchSummary()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }

            return PartialView();
        }
        public PartialViewResult PartialResources()
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
        public ActionResult ManageUsers()
        {
            if (Session["user"] == null)
            {
                this.RedirectToAction("LogOff", "Account");
            }
            var muser = (string)Session["user"];
            var myapps = (from p in Db.vw_Applications
                          where p.username != muser
                          select p).DefaultIfEmpty().ToList();
            
            return View(myapps);
        }
    }
}