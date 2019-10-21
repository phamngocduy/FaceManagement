using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;

namespace FaceManagement.Controllers
{
    public class MyClassesController : Controller
    {
        FaceIDEntities db = new FaceIDEntities();

        public JsonResult getByTag(int tagId)
        {
            var model = db.MyClasses.Where(c => c.Tag_id == tagId).ToList();
            model.ForEach(c => c.MyTag = new MyTag());
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getById(int id)
        {
            var model = db.MyClasses.Find(id);
            model.MyTag = new MyTag();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public string create(MyClass model)
        {
            if (model != null)
            {
                try
                {
                    model.date = DateTime.Now;
                    model.stop = false;
                    db.MyClasses.Add(model);
                    db.SaveChanges();
                    return "Class Created";
                }
                catch (Exception e)
                {
                    return e.GetBaseException().Message;
                }
            }
            else return "Invalid Class";
        }

        public string update(MyClass model)
        {
            if (model != null)
            {
                try
                {
                    var obj = db.MyClasses.Find(model.id);
                    obj.stop = model.stop;
                    obj.Title = model.Title;
                    db.SaveChanges();
                    return "Class Updated";
                }
                catch (Exception e)
                {
                    return e.GetBaseException().Message;
                }
            }
            else return "Invalid Class";
        }

        public string delete(int id)
        {
            try
            {
                db.MyClasses.Remove(db.MyClasses.Find(id));
                db.SaveChanges();
                return "Class Deleted";
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