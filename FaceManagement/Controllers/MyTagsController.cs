using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using FaceManagement.Models;
using System.Data;
using FastMember;

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
                    tag.User = User.Identity.Name;
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

        public FileResult download(int id)
        {
            UrlHelper url = new UrlHelper(Request.RequestContext);
            using (var workbook = new XLWorkbook())
            {
                var model = db.MyTags.Find(id);
                foreach (var item in model.MyClasses)
                {
                    var data = db.CheckIns.Where(c => c.Class_id == item.id).ToList().Select(c => new
                    {
                        Order = c.id,
                        Date = c.date,
                        Code = c.Code,
                        Image = String.Format("{0}App_Data/Checks/{1}/{2}.jpg{3}", url.Action("Index", "Home", null, Request.Url.Scheme), item.id, c.Code,
                                                c.Code != c.Email ? String.Format(" (by {0})", c.Email) : null),
                        Accuracy = c.Accuracy
                    });
                    var table = new DataTable();
                    using (var reader = ObjectReader.Create(data))
                        table.Load(reader);
                    workbook.Worksheets.Add(table, Escape(item.Title));
                }
                var memory = new MemoryStream();
                workbook.SaveAs(memory);
                return File(memory.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Escape(model.Name) + ".xlsx");
            }
        }

        private string Escape(string name)
        {
            foreach (var c in @":\/?*[]")
                name = name.Replace(c, '_');
            return name;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}