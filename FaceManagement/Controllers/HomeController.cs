using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;
using Microsoft.AspNet.SignalR;
using System.Transactions;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace FaceManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated && !String.IsNullOrEmpty((Request.Cookies["fIDLoginInfo"] ?? new HttpCookie("e")).Value))
            {
                var cookie = Request.Cookies["fIDLoginInfo"];
                var loginInfo = new ExternalLoginInfo
                {
                    Email = cookie.Values["UserEmail"],
                    DefaultUserName = cookie.Values["UserEmail"].Split('@')[0],
                    Login = new UserLoginInfo(cookie.Values["LoginProvider"], cookie.Values["ProviderKey"])
                };
                var result = HttpContext.GetOwinContext().Get<ApplicationSignInManager>().ExternalSignIn(loginInfo, isPersistent: true);
                switch (result)
                {
                    case SignInStatus.Success:
                        cookie.Expires = DateTime.Now.AddDays(365);
                        Response.SetCookie(cookie);
                        return RedirectToAction("Index");
                    default:
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.SetCookie(cookie);
                        break;
                }
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [System.Web.Mvc.Authorize]
        public ActionResult Contact()
        {
            return View();
        }

        public string Upload(string code, string user, HttpPostedFileBase face, CheckIn model, string friend, double distance = 1)
        {
            try
            {
                code = code.Split('-')[0];
                using (var db = new FaceIDEntities())
                //using (var scope = new TransactionScope())
                {
                    var @class = db.MyClasses.Find(int.Parse(code));
                    if (@class == null)
                        throw new Exception("Class doesn't exist");
                    if (Distance(@class.Latitude, @class.Longitude, model.Latitude, model.Longitude) > 1)
                        throw new Exception("You are not in class");

                    Checkin(db, int.Parse(code), user, model.Latitude, model.Longitude, user, model.Accuracy);
                    db.SaveChanges();
                    var path = "~/App_Data/Checks/";
                    Directory.CreateDirectory(Path.Combine(Server.MapPath(path), code));
                    face.SaveAs(Path.Combine(Server.MapPath(path), code, user + ".jpg"));
                    GlobalHost.ConnectionManager.GetHubContext<CheckHub>().Clients.All.addNewCheckToPage(code, user);

                    if (!String.IsNullOrEmpty(friend) && distance < 1)
                    {
                        Checkin(db, int.Parse(code), friend, model.Latitude, model.Longitude, user, distance);
                        db.SaveChanges();
                        face.SaveAs(Path.Combine(Server.MapPath(path), code, friend + ".jpg"));
                        GlobalHost.ConnectionManager.GetHubContext<CheckHub>().Clients.All.addNewCheckToPage(code, friend);
                    }
                    //scope.Complete();
                    return String.Format("Check in successfully @ {0} - {1}", @class.MyTag.Name, @class.Title);
                }
            } catch (Exception e)
            {
                return "KO:" + e.GetBaseException().Message;
            }
        }

        private void Checkin(FaceIDEntities db, int class_id, string code, double latitude, double longitude, string image, double accuracy)
        {
            var model = db.CheckIns.SingleOrDefault(c => c.Class_id == class_id && c.Code == code);
            if (model == null)
            {
                model = new CheckIn
                {
                    date = DateTime.Now,
                    Class_id = class_id,
                    Code = code,
                    Latitude = latitude,
                    Longitude = longitude,
                    Image = image,
                    Accuracy = accuracy
                };
                db.CheckIns.Add(model);
            }
            else
            {
                model.date = DateTime.Now;
                model.Latitude = model.Latitude;
                model.Longitude = model.Longitude;
                model.Image = image;
                model.Accuracy = model.Accuracy;
                db.Entry(model).State = EntityState.Modified;
            }
        }

        private double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 == lat2 && lon1 == lon2)
                return 0;
            else
            {
                var radlat1 = Math.PI * lat1 / 180;
                var radlat2 = Math.PI * lat2 / 180;
                var theta = lon1 - lon2;
                var radtheta = Math.PI * theta / 180;
                var dist = Math.Sin(radlat1) * Math.Sin(radlat2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Cos(radtheta);
                if (dist > 1) dist = 1;
                dist = Math.Acos(dist);
                dist = dist * 180 / Math.PI;
                dist = dist * 60 * 1.1515;
                return dist * 1.609344;
            }
        }

        public ViewResult Display(string id)
        {
            using (var db = new FaceIDEntities())
            {
                ViewBag.MyClass = db.MyClasses.Find(int.Parse(id)) ?? new MyClass();
                try
                {
                    var path = "~/App_Data/Checks/";
                    //var files = Directory.GetFiles(Path.Combine(Server.MapPath(path), id));
                    var files = new DirectoryInfo(Path.Combine(Server.MapPath(path), id))
                        .GetFiles().OrderByDescending(f => f.CreationTime).ToArray();
                    return View(files.Select(f => f.FullName).ToArray());
                }
                catch (Exception)
                {
                    return View(new string[0]);
                }
            }
        }
    }
}