using DataMining.DataAccess;
using DataMining.Infrastructure;
using DataMining.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace DataMining.Controllers.Api
{
    [BasicAuthentication]
    [RoutePrefix("api/identity")]
    public class IPRSController : ApiController
    {
        public string localpath = "https://neaims.go.ke/";
        MyDataEntities Db = new MyDataEntities();
        [HttpGet]
        [Route("{user}/{id}")]
        public IHttpActionResult Get(string user, string id)
        {
            var entity = (from p in Db.People
                          where p.ID_Number == id
                          select p).SingleOrDefault();
            
            var muser = user;

            var mRecord = Db.APILogins.Where(p => p.username == muser && p.active == "1").SingleOrDefault();
            if (mRecord != null)
            {
                if (entity != null)
                {
                    try
                    {
                        // Respond to JSON requests
                        SaveSearch(muser, id, "1");
                        return Content(HttpStatusCode.OK, entity, Configuration.Formatters.JsonFormatter);
                    }
                    catch
                    {
                        SaveSearch(muser, id, "0");
                        return Content(HttpStatusCode.BadRequest, entity);
                    }
                }
                else
                {
                    var myentity = GetIprsJson(id);   //Fetch from IPRS since not already stored locally
                    if (myentity != null)
                    {
                        try
                        {
                            // Respond to JSON requests
                            //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                            Person routes_list = JsonConvert.DeserializeObject<Person>(myentity);

                            if (routes_list.ID_Number != "")
                            {
                                SaveSearch(muser, id, "2");
                                return Content(HttpStatusCode.OK, routes_list, Configuration.Formatters.JsonFormatter);
                            }
                            else
                            {
                                SaveSearch(muser, id, "5");
                                return Content(HttpStatusCode.BadRequest, myentity);
                            }
                            
                        }
                        catch
                        {

                            SaveSearch(muser, id, "0");
                            return Content(HttpStatusCode.BadRequest, myentity);
                        }
                    }
                    else
                    {
                        SaveSearch(muser, id, "0");
                        return Content(HttpStatusCode.BadRequest, "No Business with ID = " + id.ToString() + " Defined in Chengsee Systems Platform!!");
                    }
                }
            }
            else
            {
                SaveSearch(muser, id, "9");
                return Content(HttpStatusCode.BadRequest, "Request not Defined in Chengsee Systems Platform!!");
            }
            
        }

        public void SaveSearch(string muser, string midno, string mfound)
        {
            Db.Person_Search_Log.Add(new Person_Search_Log()
            {
                ID_Number = midno,
                username = muser,
                Found = mfound,
                DateCreated = DateTime.Now
            });

            Db.SaveChanges();
        }

        [HttpGet]
        [Route("iprs/{id}")]
        public IHttpActionResult GetIprs(string id)
        {
            IPRSViewModel bizness = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(localpath);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var responseTask = client.GetAsync("genapi/api/IPRS/GetPersonByID/" + id.ToString());
                try
                {
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IPRSViewModel>();
                        readTask.Wait();

                        bizness = readTask.Result;

                    }
                }
                catch { }

                using (MyDataEntities Db = new MyDataEntities())
                {
                    if (bizness != null)
                    {
                        if (bizness.ErrorOcurred == false)
                        {
                            var query = (from p in Db.People
                                         where p.ID_Number == id
                                         select p).SingleOrDefault();
                            if (query == null)
                            {
                                Db.Person_Tmp.Add(new Person_Tmp()
                                {
                                    ID_Number = id,
                                    Surname = bizness.Surname,
                                    First_Name = bizness.First_Name,
                                    Other_Name = bizness.Other_Name,
                                    Gender = bizness.Gender,
                                    Date_of_Birth = bizness.Date_of_Birth,
                                    Citizenship = bizness.Citizenship,
                                    Clan = bizness.Clan,
                                    Ethnic_Group = bizness.Ethnic_Group,
                                    Family = bizness.Family,
                                    Serial_Number = bizness.Serial_Number,
                                    Place_of_Birth = bizness.Place_of_Birth,
                                    Place_of_Live = bizness.Place_of_Live
                                });

                                Db.SaveChanges();
                            }
                        }
                    }

                    var query2 = (from p in Db.Person_IDs
                                  where p.ID_Number == id
                                  select p).SingleOrDefault();
                    if (query2 != null)
                    {
                        query2.Processed = true;
                        Db.SaveChanges();
                    }
                }
            }
            return Content(HttpStatusCode.OK, bizness, Configuration.Formatters.JsonFormatter);
        }

        public string GetIprsJson(string id)
        {
            IPRSViewModel bizness = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(localpath);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var responseTask = client.GetAsync("genapi/api/IPRS/GetPersonByID/" + id.ToString());
                try
                {
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IPRSViewModel>();
                        readTask.Wait();

                        bizness = readTask.Result;

                    }
                }
                catch { }

                using (MyDataEntities Db = new MyDataEntities())
                {
                    if (bizness != null)
                    {
                        if (bizness.ErrorOcurred == false)
                        {
                            var query = (from p in Db.People
                                         where p.ID_Number == id
                                         select p).SingleOrDefault();
                            if (query == null)
                            {
                                Db.Person_Tmp.Add(new Person_Tmp()
                                {
                                    ID_Number = id,
                                    Surname = bizness.Surname,
                                    First_Name = bizness.First_Name,
                                    Other_Name = bizness.Other_Name,
                                    Gender = bizness.Gender,
                                    Date_of_Birth = bizness.Date_of_Birth,
                                    Citizenship = bizness.Citizenship,
                                    Clan = bizness.Clan,
                                    Ethnic_Group = bizness.Ethnic_Group,
                                    Family = bizness.Family,
                                    Serial_Number = bizness.Serial_Number,
                                    Place_of_Birth = bizness.Place_of_Birth,
                                    Place_of_Live = bizness.Place_of_Live
                                });

                                Db.SaveChanges();
                            }
                        }
                    }
                    
                }
            }

            //Convert List Into JSON
            var jsonString = JsonConvert.SerializeObject(bizness, Converter.Settings);
            
            return jsonString;
        }
        
        [HttpGet]
        [Route("millionfrom/{idno}")]
        public IHttpActionResult GetMillion(int idno)
        {
            IPRSViewModel bizness = null;
            string id = idno.ToString();
            int imax = idno + 1000000;

            for (int i = idno; i < imax; i++)
            {
                var query = (from p in Db.People
                             where p.ID_Number == id
                             select p).SingleOrDefault();
                if (query == null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(localpath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //HTTP GET
                        var responseTask = client.GetAsync("genapi/api/IPRS/GetPersonByID/" + id.ToString());
                        try
                        {
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<IPRSViewModel>();
                                readTask.Wait();

                                bizness = readTask.Result;

                            }
                        }
                        catch { }

                        using (MyDataEntities Db = new MyDataEntities())
                        {
                            if (bizness != null)
                            {
                                if (bizness.ErrorOcurred == false)
                                {
                                    Db.Person_Tmp.Add(new Person_Tmp()
                                    {
                                        ID_Number = id,
                                        Surname = bizness.Surname,
                                        First_Name = bizness.First_Name,
                                        Other_Name = bizness.Other_Name,
                                        Gender = bizness.Gender,
                                        Date_of_Birth = bizness.Date_of_Birth,
                                        Citizenship = bizness.Citizenship,
                                        Clan = bizness.Clan,
                                        Ethnic_Group = bizness.Ethnic_Group,
                                        Family = bizness.Family,
                                        Serial_Number = bizness.Serial_Number,
                                        Place_of_Birth = bizness.Place_of_Birth,
                                        Place_of_Live = bizness.Place_of_Live
                                    });

                                    Db.SaveChanges();
                                }
                            }
                        }
                    }
                }
                id = i.ToString();
            }
            return Content(HttpStatusCode.OK, bizness, Configuration.Formatters.JsonFormatter);
        }


        [HttpGet]
        [Route("pending")]
        public IHttpActionResult GetPending()
        {
            var querydata = (from p in Db.Person_IDs
                         where p.Processed == null
                         select p).ToList();

            foreach (var iden in querydata)
            {
                string idno = iden.ID_Number;
                IPRSViewModel bizness = null;
                string id = idno.ToString();
                var query = (from p in Db.People
                             where p.ID_Number == id
                             select p).SingleOrDefault();
                if (query == null)
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(localpath);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //HTTP GET
                        var responseTask = client.GetAsync("genapi/api/IPRS/GetPersonByID/" + id.ToString());
                        try
                        {
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<IPRSViewModel>();
                                readTask.Wait();

                                bizness = readTask.Result;

                            }
                        }
                        catch { }

                        using (MyDataEntities Db = new MyDataEntities())
                        {
                            if (bizness != null)
                            {
                                if (bizness.ErrorOcurred == false)
                                {
                                    Db.Person_Tmp.Add(new Person_Tmp()
                                    {
                                        ID_Number = id,
                                        Surname = bizness.Surname,
                                        First_Name = bizness.First_Name,
                                        Other_Name = bizness.Other_Name,
                                        Gender = bizness.Gender,
                                        Date_of_Birth = bizness.Date_of_Birth,
                                        Citizenship = bizness.Citizenship,
                                        Clan = bizness.Clan,
                                        Ethnic_Group = bizness.Ethnic_Group,
                                        Family = bizness.Family,
                                        Serial_Number = bizness.Serial_Number,
                                        Place_of_Birth = bizness.Place_of_Birth,
                                        Place_of_Live = bizness.Place_of_Live
                                    });

                                    Db.SaveChanges();
                                }
                            }
                        }

                        var query2 = (from p in Db.Person_IDs
                                      where p.ID_Number == id
                                      select p).SingleOrDefault();
                        if (query2 != null)
                        {
                            query2.Processed = true;
                            Db.SaveChanges();
                        }
                    }
                }
            }

            return Content(HttpStatusCode.OK, "Data Processed Complete!!!", Configuration.Formatters.JsonFormatter);
        }
    }
}
