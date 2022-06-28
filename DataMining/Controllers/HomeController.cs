using DataMining.DataAccess;
using DataMining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace DataMining.Controllers
{
    public class HomeController : Controller
    {
        public string localpath = "https://10.190.4.233/getdata";
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        //public ActionResult Pending()
        //{
        //    ViewBag.Title = "Pending Data";
        //    using (MyDataEntities Db = new MyDataEntities())
        //    {
        //        var query = (from p in Db.Person_IDs
        //                     where p.Processed == null
        //                     select p).ToList();
        //        //var highestId = query.Any() ? query.Max(x => x.ID) : 0;
        //        var maxId = Db.Person_IDs.Count(x => x.Processed == null);

        //        int maxValue = maxId;
        //        int i = 0;
        //        //maxValue += 1;
        //        foreach (var iden in query)
        //        {
        //            string idno = iden.ID_Number;
        //            using (var client = new HttpClient())
        //            {
        //                client.BaseAddress = new Uri(localpath);
        //                client.DefaultRequestHeaders.Accept.Clear();
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //                //HTTP GET
        //                var responseTask = client.GetAsync("api/identity/" + idno);

        //                responseTask.Wait();

        //                var result = responseTask.Result;
        //                if (result.IsSuccessStatusCode)
        //                {
        //                    var readTask = result.Content.ReadAsAsync<IPRSViewModel>();
        //                    readTask.Wait();
        //                }
        //                i += 1;
        //            }
        //        }
        //        ViewBag.Records = "Data Processed = " + i.ToString();
        //        return View();
        //    }
        //}
        public ActionResult About()
        {
            ViewBag.Title = "About Page";

            return View();
        }

        public ActionResult Identifier()
        {
            ViewBag.Title = "Identifier Page";

            return View();
        }

        public ActionResult Trusted()
        {
            ViewBag.Title = "Trusted Service Page";

            return View();
        }
        public ActionResult CardSystem()
        {
            ViewBag.Title = "Smart Cards";

            return View();
        }
    }
}
