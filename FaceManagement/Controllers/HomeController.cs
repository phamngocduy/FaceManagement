using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;

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

        public ActionResult Contact()
        {
            return View();
        }

        public string Upload(string code, string user, HttpPostedFileBase face)
        {
            try
            {
                code = code.Split('-')[0];
                using (var db = new FaceIDEntities())
                    if (db.MyClasses.Find(int.Parse(code)) == null)
                        throw new Exception("Class doesn't exist");
                var path = "~/App_Data/Checks/";
                Directory.CreateDirectory(Path.Combine(Server.MapPath(path), code));
                face.SaveAs(Path.Combine(Server.MapPath(path), code, user + ".jpg"));
                return "Check in successfully";

            } catch (Exception e)
            {
                return e.GetBaseException().Message;
            }
        }
    }
}