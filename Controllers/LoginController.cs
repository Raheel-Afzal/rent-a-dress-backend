using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using FYPAPI.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace FYPAPI.Controllers
{

    public class LoginController : ApiController
    {
        Entities1 db = new Entities1();

        //ShowAllUser
        



        [HttpGet]
        
        public HttpResponseMessage showUsers()
        {
            try
            {
                var data = db.USERINFOes.ToList();
                if (data != null)
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid User");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage savereco(RECOMEND reco)
        {
            if (reco == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "not found");
            }
            else {
                db.RECOMENDs.Add(reco);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "ADDED");
            }



        }
        //Login
        [HttpGet]
        public HttpResponseMessage Login(string email, string pass)
        {
            try
            {

                var data = db.USERINFOes.Where(x => x.email == email && x.password == pass).FirstOrDefault();
                if (data != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //Get only Dressname
        [HttpGet]
        public HttpResponseMessage getdressname() {
            try
            {

                var dressNames = db.DRESSINFOes.ToList();
                if (dressNames != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, dressNames);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Invalid User");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //showdresswithid
        [HttpGet]
        public HttpResponseMessage showdresswithid(int i)
        {
            try
            {
                var user = db.DRESSINFOes.Where(b => b.uid == i).ToList();

                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.did).ToList();
                }

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //showdresswithid
        [HttpGet]
        public HttpResponseMessage showrent(int i)
        {
            try
            {
                var user = db.RENTs.Where(b => b.renterid == i).ToList();


                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
                else
                {
                    foreach (var info in user)
                    {
                        info.Reqname = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.name).FirstOrDefault();
                        info.Reqcontact = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.contact).FirstOrDefault();
                        info.Reqaddress = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.address).FirstOrDefault();
                        info.Reqcity = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.city).FirstOrDefault();



                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //Dressrentrequest
        [HttpGet]
        public HttpResponseMessage showrenter(int uid)
        {
            try
            {
                var user = (from r in db.RENTs
                            join d in db.DRESSINFOes on r.dressid equals d.did
                            join s in db.DRESSIMAGEs on d.did equals s.dressid
                            join u in db.USERINFOes on r.renterid equals u.id
                            where u.id == uid
                            select new
                            {
                                r.renterid,
                                r.dressid,
                                r.rentstartdate,
                                r.rentenddate,
                                r.pickingdate,
                                r.requeststatus,
                                u.name,
                                s.dressimage1
                            }).ToList();

                if (user == null || user.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
                else
                {


                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //uploadimage
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult uploaddressImage(int id)
        {
            DRESSIMAGE d = new DRESSIMAGE();

            var form = HttpContext.Current.Request.Form;

            var files = HttpContext.Current.Request.Files;

            //string dateTimeStamp = DateTime.Now.ToString();
            string path = HttpContext.Current.Server.MapPath("~/Images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            files[0].SaveAs(path + "/" + files[0].FileName);

            d.dressimage1 = files[0].FileName;
            d.dressid = id;
            db.DRESSIMAGEs.Add(d);
            db.SaveChanges();

            return Ok(d);


        }

        //SaveUSer
        [HttpPost]
        public HttpResponseMessage saveuser(USERINFO u)
        {
            try
            {
                db.USERINFOes.Add(u);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, u);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
      
        //upload Multiple pic
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult uploadMultipleDressImage()
        {
            DRESSIMAGE d = new DRESSIMAGE();

            var form = HttpContext.Current.Request.Form;

            String i = form["uid"];
            int uid = int.Parse(i);
            String name = form["name"];
            String type = form["type"];
            String description = form["descrip"];
            String r = form["rent"];
            int rent = int.Parse(r);
            String size = form["size"];
            String color = form["color"];
            String gender = form["gender"];
            String quality = form["quality"];
            String status = form["status"];


            DRESSINFO dd = new DRESSINFO();

            dd.uid = uid;
            dd.name = name;
            dd.type = type;
            dd.rent = rent;
            dd.size = size;
            dd.color = color;
            dd.descriptin = description;
            dd.geneder = gender;
            dd.quality = quality;
            dd.status = status;
            dd.rating = 0.0;
            db.DRESSINFOes.Add(dd);
            db.SaveChanges(); 






            var files = HttpContext.Current.Request.Files;
            string path = HttpContext.Current.Server.MapPath("~/images");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

            }
            var listdress = new List<Object>();
            for (int j = 0; j < files.Count; j++)
            {
                var filename = DateTime.Now.Ticks.ToString() + files[j].FileName;


                files[j].SaveAs(path + "/" + filename);

                d.dressid = dd.did;

                d.dressimage1 = filename;
                db.DRESSIMAGEs.Add(d);
                db.SaveChanges();


            }


            return Ok("uploaded");

        }
        //Delete Dress
        [HttpPost]
        public HttpResponseMessage DeleteDress(int id)
        {
            var existingDressInfo = db.DRESSINFOes.Find(id);

            if (existingDressInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Dress Not Found");
            }

            // Delete the dress images
            var dressImages = db.DRESSIMAGEs.Where(d => d.dressid == id);

            foreach (var image in dressImages)
            {
                string imagePath = HttpContext.Current.Server.MapPath("~/images/" + image.dressimage1);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                db.DRESSIMAGEs.Remove(image);
            }

            // Delete the dress detail
            db.DRESSINFOes.Remove(existingDressInfo);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "Dress and associated images deleted successfully");
        }

        //Update Dress Detail
        [HttpPost]
        public HttpResponseMessage updatedress()
        {

            var form = HttpContext.Current.Request.Form;


            String i = form["did"];
            int did = int.Parse(i);

            var existingDressInfo = db.DRESSINFOes.Find(did);

            if (existingDressInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Not Found");
            }


            String name = form["name"];
            String type = form["type"];
            String r = form["rent"];
            int rent = int.Parse(r);
            String size = form["size"];
            String color = form["color"];
            String quality = form["quality"];


            existingDressInfo.name = name;
            existingDressInfo.type = type;
            existingDressInfo.rent = rent;
            existingDressInfo.size = size;
            existingDressInfo.color = color;
            existingDressInfo.quality = quality;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "DRESSINFO updated successfully");

        }

        //showdresses
        [HttpGet]
        public HttpResponseMessage showdresses()
        {
            try
            {
                var dress = db.DRESSINFOes.ToList();
                foreach (var item in dress)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.did).ToList();
                }

                if (dress != null)
                {
                    foreach (var info in dress)
                    {
                        info.Oid = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.id).FirstOrDefault();
                        info.Oname = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.name).FirstOrDefault();
                        info.Contact = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.contact).FirstOrDefault();
                        info.Address = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.address).FirstOrDefault();
                        info.City = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.city).FirstOrDefault();
                        info.status = string.IsNullOrEmpty(info.status) ? "Available" : info.status;

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, dress);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //Savedress
        [HttpPost]
        public HttpResponseMessage savedress(DRESSINFO u)
        {
            try
            {
                db.DRESSINFOes.Add(u);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, u);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //showWishlist
        [HttpGet]
        public HttpResponseMessage showwishlist()
        {
            try
            {
                var user = db.WHISHLISTs.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //SaveWishlist
        [HttpPost]
        public HttpResponseMessage savewishlist(WHISHLIST u)
        {
            try
            {
                db.WHISHLISTs.Add(u);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "Wishlist with id : " + u.wislist_id + "saved");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //showrent
        [HttpGet]
        public HttpResponseMessage showrent()
        {
            try
            {
                var user = db.RENTs.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        //showtoowner
        [HttpGet]
        public HttpResponseMessage showtoowner(int uid)
        {
            try
            {
                String s = "Pending";
                var user = db.RENTs.Where(x => x.oid == uid && x.requeststatus == s).ToList();
                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user != null)
                {
                    foreach (var info in user)
                    {

                        info.Reqname = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.name).FirstOrDefault();
                        info.Reqcontact = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.contact).FirstOrDefault();
                        info.Reqaddress = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.address).FirstOrDefault();
                        info.Reqcity = db.USERINFOes.Where(x => x.id == info.renterid).Select(y => y.city).FirstOrDefault();


                    }
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Not Found");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //showtorenter
        [HttpGet]
        public HttpResponseMessage showtorenter(int oid)

        {
            try
            {
                var user = db.RENTs.Where(x => x.renterid == oid).ToList();
                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user != null)
                {
                    foreach (var info in user)
                    {

                        info.Reqname = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.name).FirstOrDefault();
                        info.Reqcontact = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.contact).FirstOrDefault();
                        info.Reqaddress = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.address).FirstOrDefault();
                        info.Reqcity = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.city).FirstOrDefault();


                    }
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Not Found");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //SaveRent
        [HttpPost]
        public HttpResponseMessage saverent(RENT u)
        {
            try
            {
                db.RENTs.Add(u);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, u);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //SaveRequeststatus
        [HttpPost]
        public HttpResponseMessage savestatus(int uid, int oid, int did, String s, String h)
        {
            try
            {
                var uu = JsonConvert.DeserializeObject<HISTORY>(h);
                var user = db.RENTs.Where(x => x.renterid == uid && x.oid == oid && x.dressid == did).FirstOrDefault();

                var ds = db.DRESSINFOes.Where(y => y.uid == oid && y.did == did).FirstOrDefault();

                if (user != null)
                {
                    user.requeststatus = s;
                    if (s.Equals("Accepted"))
                    {
                        ds.status = "Not Available";
                        db.HISTORies.Add(uu);
                    }
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "Not found");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //SaveRequeststatus
        [HttpPost]
        public HttpResponseMessage savestatuss(int uid, int oid, int did, String s)
        {
            try
            {
                var user = db.RENTs.Where(x => x.renterid == uid && x.oid == oid && x.dressid == did).FirstOrDefault();

                var ds = db.DRESSINFOes.Where(y => y.uid == oid && y.did == did).FirstOrDefault();

                if (user != null)
                {
                    user.requeststatus = s;
                    if (s.Equals("Accept"))
                    {
                        ds.status = "Not Available";
                    }
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "Not found");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        //Returndress
        [HttpGet]
        public HttpResponseMessage returndress(int oid)

        {
            try
            {
                String s = "Accepted";
                var user = db.RENTs.Where(x => x.renterid == oid && x.requeststatus == s).ToList();
                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user != null)
                {
                    foreach (var info in user)
                    {

                        info.Reqname = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.name).FirstOrDefault();
                        info.Reqcontact = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.contact).FirstOrDefault();
                        info.Reqaddress = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.address).FirstOrDefault();
                        info.Reqcity = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.city).FirstOrDefault();


                    }
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Not Found");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //showtorenter
        [HttpGet]
        public HttpResponseMessage history(int oid)

        {
            try
            {
                var user = db.HISTORies.Where(x => x.renterid == oid || x.oid == oid).ToList();
                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user != null)
                {
                    foreach (var info in user)
                    {

                        info.Reqname = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.name).FirstOrDefault();
                        info.Reqcontact = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.contact).FirstOrDefault();
                        info.Reqaddress = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.address).FirstOrDefault();
                        info.Reqcity = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.city).FirstOrDefault();
                        if (oid == info.renterid)
                        {
                            info.Reqstatus = "Rented";
                        }
                        else if (oid == info.oid) {
                            info.Reqstatus = "Rented Out";
                        }

                    }
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                return Request.CreateResponse(HttpStatusCode.OK, "Not Found");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        //showFavt
        [HttpGet]
        public HttpResponseMessage showfavt(int oid)
        {
            try
            {
                var user = db.OWNERFAVORITEs.Where(b => b.userid == oid).ToList();




                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
                else
                {
                    foreach (var info in user)
                    {
                        info.Oname = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.name).FirstOrDefault();
                        info.Contact = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.contact).FirstOrDefault();
                        info.Address = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.address).FirstOrDefault();
                        info.City = db.USERINFOes.Where(x => x.id == info.oid).Select(y => y.city).FirstOrDefault();



                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }





        //showwishlist
        [HttpGet]
        public HttpResponseMessage wishlist(int oid)
        {
            try
            {
                var user = db.WHISHLISTs.Where(b => b.requesterid == oid).ToList();


                foreach (var item in user)
                {
                    item.images = db.DRESSIMAGEs.Where(x => x.dressid == item.dressid).ToList();
                }

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
                else
                {
                    foreach (var info in user)
                    {
                        int ooid = db.DRESSINFOes.Where(x => x.did == info.dressid).Select(y => y.uid).FirstOrDefault();
                        info.Dname = db.DRESSINFOes.Where(x => x.did == info.dressid).Select(y => y.name).FirstOrDefault();
                        info.Status = db.DRESSINFOes.Where(x => x.did == info.dressid).Select(y => y.status).FirstOrDefault();
                        info.Oname = db.USERINFOes.Where(x => x.id == ooid).Select(y => y.name).FirstOrDefault();
                        info.Contact = db.USERINFOes.Where(x => x.id == ooid).Select(y => y.contact).FirstOrDefault();
                        info.Address = db.USERINFOes.Where(x => x.id == ooid).Select(y => y.address).FirstOrDefault();
                        info.City = db.USERINFOes.Where(x => x.id == ooid).Select(y => y.city).FirstOrDefault();



                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet]

        public HttpResponseMessage showuser()
        {
            try
            {
                var user = db.DRESSINFOes.ToList();
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not found");
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        //datesearchshow








        //Function to check if a dress is available between two dates
        public bool chkdate(int id, string startdate, string enddate)
        {
            DateTime startDateTime, endDateTime;

            if (!DateTime.TryParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDateTime))
            {
                return false; // invalid start date format
            }

            if (!DateTime.TryParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDateTime))
            {
                return false; // invalid end date format
            }

            var history = db.HISTORies.Where(x => x.dressid == id)
                .ToList() // fetch data from DB and load into memory
                .Where(x =>
                {
                    DateTime startDate, endDate;
                    if (!DateTime.TryParseExact(x.rentstartdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    {
                        return false; // invalid start date format in history table
                    }

                    if (!DateTime.TryParseExact(x.rentenddate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    {
                        return false; // invalid end date format in history table
                    }

                    return (startDateTime >= startDate && startDateTime <= endDate) || (endDateTime >= startDate && endDateTime <= endDate);
                })
                .ToList();

            if (history.Count > 0)
                return false; //not available
            else
                return true; //available
        }


        //datesearchwithid
        [HttpGet]
        public HttpResponseMessage showdate( string s, string e)
        {
            try
            {
                var dress = db.DRESSINFOes.ToList();
                foreach (var item in dress)
                {
                    item.images = db.DRESSIMAGEs.ToList();
                }

                if (dress != null)
                {
                    foreach (var info in dress)
                    {
                        info.Oid = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.id).FirstOrDefault();
                        info.Oname = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.name).FirstOrDefault();
                        info.Contact = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.contact).FirstOrDefault();
                        info.Address = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.address).FirstOrDefault();
                        info.City = db.USERINFOes.Where(x => x.id == info.uid).Select(y => y.city).FirstOrDefault();
                        info.status = string.IsNullOrEmpty(info.status) ? "Available" : info.status;
                    }

                    var filteredDress = new List<DRESSINFO>();
                    foreach (var info in dress)
                    {
                        if (chkdate(info.did, s, e))
                        {
                            filteredDress.Add(info);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, filteredDress);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        
        //Saverating
        [HttpPost]
        public HttpResponseMessage saverating(Double drating, Double orating, int did, int oid ,int uid, String h)
        {
            try
            {

                var user = db.USERINFOes.FirstOrDefault(x => x.id == oid);
                if (user != null)
                {
                    double? b = user.orating;
                    if (b == null || b == 0)
                    {
                        user.orating = orating;
                    }
                    else
                    {
                        b = (b + orating) / 2;
                        user.orating = b;
                    }
                }

                var ds = db.DRESSINFOes.Where(y => y.uid == oid && y.did == did).FirstOrDefault();
                var d = db.RENTs.Where(y => y.oid == oid && y.dressid == did).FirstOrDefault();
                if (ds != null)
                {
                    double? b = ds.rating;
                    if (b == null || b == 0)
                    {
                        ds.rating = drating;
                    }
                    else
                    {
                        b = (b + drating) / 2;
                        ds.rating = b;
                    }
                }

                if (user != null)
                {
                    d.requeststatus = "Return";
                    //  ds.status = "Not Available";
                    if (h.Equals("Y"))
                    {
                        OWNERFAVORITE uu = new OWNERFAVORITE();
                        uu.oid= oid;
                        uu.userid= uid;
                        db.OWNERFAVORITEs.Add(uu);
                    }
                   
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                    return Request.CreateResponse(HttpStatusCode.OK, "Not found");

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }





    }
}
