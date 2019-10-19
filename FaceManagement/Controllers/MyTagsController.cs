using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;

namespace FaceManagement.Controllers
{
    public class MyTagsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getAll()
        {
            using (var db = new FaceIDEntities())
            {
                var tags = db.MyTags.ToList();
                return Json(tags, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getById(int id)
        {
            using (var db = new FaceIDEntities())
            {
                var tag = db.MyTags.Find(id);
                return Json(tag, JsonRequestBehavior.AllowGet);
            }
        }

        public string create(MyTag tag)
        {
            if (tag != null)
            {
                using (var db = new FaceIDEntities())
                    try
                    {
                        db.MyTags.Add(tag);
                        db.SaveChanges();
                        return "Tag Created";
                    }
                    catch (Exception e)
                    {
                        return e.GetBaseException().Message;
                    }
            }
            else return "Invalid Tag";
        }

        public string update(MyTag model)
        {
            if (model != null)
            {
                using (var db = new FaceIDEntities())
                try
                {
                    var tag = db.MyTags.Find(model.id);
                    tag.Name = model.Name;
                    db.SaveChanges();
                    return "Tag Updated";
                }
                catch (Exception e)
                {
                    return e.GetBaseException().Message;
                }
            }
            else return "Invalid Tag";
        }
    }
}