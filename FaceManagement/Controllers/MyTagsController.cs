using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;

namespace FaceManagement.Controllers
{
    [Authorize]
    public class MyTagsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        FaceIDEntities db = new FaceIDEntities();

        public JsonResult getAll()
        {
            var tags = db.MyTags.Where(t => t.User == User.Identity.Name).ToList();
            tags.ForEach(t => t.MyClasses.Clear());
            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getById(int id)
        {
            var tag = db.MyTags.Find(id);
            tag.MyClasses.Clear();
            return Json(tag, JsonRequestBehavior.AllowGet);
        }

        public string create(MyTag tag)
        {
            if (tag != null)
            {
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

        public string delete(int id)
        {
            try
            {
                var tag = db.MyTags.Find(id);
                db.MyClasses.RemoveRange(tag.MyClasses);
                db.MyTags.Remove(tag);
                db.SaveChanges();
                return "Tag Deleted";
            }
            catch (Exception e)
            {
                return e.GetBaseException().Message;
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}