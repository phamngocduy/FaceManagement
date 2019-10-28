using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FaceManagement.Models;

namespace FaceManagement.Controllers
{
    public class ErrorLogsController : Controller
    {
        private FaceIDEntities db = new FaceIDEntities();

        // GET: ErrorLogs
        public async Task<ActionResult> Index()
        {
            return View(await db.ErrorLogs.ToListAsync());
        }

        // GET: ErrorLogs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ErrorLog errorLog = await db.ErrorLogs.FindAsync(id);
            if (errorLog == null)
            {
                return HttpNotFound();
            }
            return View(errorLog);
        }

        // GET: ErrorLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ErrorLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(ErrorLog errorLog)
        {
            if (ModelState.IsValid)
            {
                db.ErrorLogs.Add(errorLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(errorLog);
        }

        // GET: ErrorLogs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ErrorLog errorLog = await db.ErrorLogs.FindAsync(id);
            if (errorLog == null)
            {
                return HttpNotFound();
            }
            return View(errorLog);
        }

        // POST: ErrorLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,Code,Error,Trace")] ErrorLog errorLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(errorLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(errorLog);
        }

        // GET: ErrorLogs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ErrorLog errorLog = await db.ErrorLogs.FindAsync(id);
            if (errorLog == null)
            {
                return HttpNotFound();
            }
            return View(errorLog);
        }

        // POST: ErrorLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ErrorLog errorLog = await db.ErrorLogs.FindAsync(id);
            db.ErrorLogs.Remove(errorLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
