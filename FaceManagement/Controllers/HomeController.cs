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

namespace FaceManagement.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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

        public string Upload(string code, string user, HttpPostedFileBase face, CheckIn model)
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
                    if (distance(@class.Latitude, @class.Longitude, model.Latitude, model.Longitude) > 1)
                        throw new Exception("You are not in class");
                    var check = db.CheckIns.SingleOrDefault(c => c.Class_id == @class.id && c.Code == user);
                    if (check == null)
                    {
                        model.date = DateTime.Now;
                        model.Class_id = @class.id;
                        model.Code = model.Image = user;
                        db.CheckIns.Add(model);
                    } else
                    {
                        check.date = DateTime.Now;
                        check.Code = check.Image = user;
                        check.Latitude = model.Latitude;
                        check.Longitude = model.Longitude;
                        check.Accuracy = model.Accuracy;
                        db.Entry(check).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    var path = "~/App_Data/Checks/";
                    Directory.CreateDirectory(Path.Combine(Server.MapPath(path), code));
                    face.SaveAs(Path.Combine(Server.MapPath(path), code, user + ".jpg"));

                    //scope.Complete();
                    GlobalHost.ConnectionManager.GetHubContext<CheckHub>().Clients.All.addNewCheckToPage(code, user);
                    return String.Format("Check in successfully @ {0} - {1}", @class.MyTag.Name, @class.Title);
                }
            } catch (Exception e)
            {
                return "KO:" + e.GetBaseException().Message;
            }
        }

        private double distance(double lat1, double lon1, double lat2, double lon2)
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