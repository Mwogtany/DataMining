using DataMining.DataAccess;
using DataMining.Infrastructure;
using DataMining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataMining.Controllers.Api
{
    //[BasicAuthentication]
    [RoutePrefix("api/asyncidentity")]
    public class IPRSAsyncController : ApiController
    {
        public string localpath = "https://neaims.go.ke/";
        MyDataEntities Db = new MyDataEntities();

        [HttpGet]
        [Route("{user}/{id}")]
        [ResponseType(typeof(Person))]
        public async Task<IHttpActionResult> Get(string user, string id)
        {
            if (ValidUser(user))
            {
                using (var ctx = new MyDataEntities())
                {
                    Person com = await ctx.People.FindAsync(id);

                    if (com != null)
                    {
                        SaveSearch(user, id, "1");
                        return Ok(com);
                    }
                    else
                    {
                        var com2 = Task.Run(() => GetIPRSAsync(id));  //Retrieve from IPRS
                        if (com2 != null)
                        {
                            SaveSearch(user, id, "2");
                            return Ok(com2.Result);
                        }
                        else
                        {
                            SaveSearch(user, id, "5");
                            return NotFound();
                        }
                    }

                }
            }
            else
            {
                SaveSearch(user, id, "5");
                return NotFound();
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
        //Fetch data from IPRS Platform
        [ResponseType(typeof(Person))]
        public async Task<Person> GetIPRSAsync(string id)
        {
            IPRSViewModel bizness = null;
            Person mperson = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(localpath);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HTTP GET
                var responseTask = await client.GetAsync("genapi/api/IPRS/GetPersonByID/" + id.ToString());
                try
                {
                    //responseTask.Wait();
                    var result = responseTask;
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

                            mperson = new Person()
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
                            };

                            //return Ok(mperson);
                        }
                    }

                }
            }

            //Convert List Into JSON
            //var jsonString = JsonConvert.SerializeObject(bizness, Converter.Settings);

            //return jsonString;
            return mperson;
        }

        [HttpGet]
        [Route("millionrec/{startid}/{endid}")]
        public async Task<string> GetMillion(int startid, int endid)
        {
            string id = startid.ToString();
            int imax = endid;
            string mystring = "";
            int j = 0;
            int k = 0;
            for (int i = startid; i < imax; i++)
            {
                id = i.ToString();
                using (var ctx = new MyDataEntities())
                {
                    Person com = await ctx.People.FindAsync(id);

                    if (com == null)
                    {
                        var com2 = Task.Run(() => GetIPRSAsync(id));  //Retrieve from IPRS
                        if (com2 != null)
                        {
                            k++;
                        }
                        else
                        {
                            j++;
                        }
                    }
                    else
                    {
                        k++;
                    }
                }
            }
            mystring = k.ToString() + " Were Found; " + j.ToString() + " Not Found!!!";
            return mystring;
        }

        [HttpGet]
        [Route("arec/{startid}")]
        public async Task<string> GetRec(int startid)
        {
            string id = startid.ToString();
            string mystring = "";
            using (var ctx = new MyDataEntities())
            {
                Person com = await ctx.People.FindAsync(id);

                if (com == null)
                {
                    var com2 = Task.Run(() => GetIPRSAsync(id));  //Retrieve from IPRS
                    if (com2 != null)
                    {
                        mystring = id + " ID legal Serial = " + com2.Result.Serial_Number;
                    }
                    else
                    {
                        mystring = id + " ID No Illegal";
                    }
                }
                else
                {
                    mystring = id + " ID legal Serial = " + com.Serial_Number;
                }

            }
            return mystring;
        }

        [HttpGet]
        [Route("{user}/pin/{id}")]
        [ResponseType(typeof(Person_Pin))]
        public async Task<IHttpActionResult> GetPin(string user, string id)
        {
            if (ValidUser(user))
            {
                using (var ctx = new MyDataEntities())
                {
                    Person_IDs com = await ctx.Person_IDs.FindAsync(id);

                    if (com != null)
                    {
                        var mypin = new Person_Pin()
                        {
                            ID_Number = com.ID_Number,
                            KRAPIN = com.KRAPIN
                        };

                        SaveSearch(user, id, "7");
                        return Ok(mypin);
                    }
                    else
                    {
                        SaveSearch(user, id, "8");
                        return NotFound();
                    }

                }
            }
            else
            {
                SaveSearch(user, id, "5");
                return NotFound();
            }
        }

        public static bool ValidUser(string uname)
        {
            using (MyDataEntities entity = new MyDataEntities())
            {
                //return entity.Logins.Any(x => x.username.Equals(uname, StringComparison.OrdinalIgnoreCase) && x.password == pass);

                //var mpass = model.Password;
                var varuser = (from p in entity.Logins
                               join x in entity.APILogins on p.username equals x.username
                               where p.username == uname
                               select new LoginViewModel2()
                               {
                                   Username = p.username,
                                   Role = x.UserRole,
                                   Active = x.active
                               }).FirstOrDefault();

                if (varuser != null)
                {
                    // Initialization.    
                    var mActive = varuser.Active;
                    if (mActive == "1")
                    {
                        return true;
                    }
                    else
                    {
                        // Setting.    
                        return false;
                    }
                }
                else
                {
                    // Setting.    
                    return false;
                }
            }
            return false;
        }
    }
}
