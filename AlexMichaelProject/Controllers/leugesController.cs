using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlexMichaelProject.Models;

namespace AlexMichaelProject.Controllers
{
    public class leugesController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            return View(await db.leuges.ToListAsync());
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leuge leuge = await db.leuges.FindAsync(id);
            if (leuge == null)
            {
                return HttpNotFound();
            }
            return View(leuge);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "leugeName")] leuge leuge)
        {
            if (ModelState.IsValid)
            {
                db.leuges.Add(leuge);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(leuge);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leuge leuge = await db.leuges.FindAsync(id);
            if (leuge == null)
            {
                return HttpNotFound();
            }
            return View(leuge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "leugeName")] leuge leuge)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leuge).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(leuge);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            leuge leuge = await db.leuges.FindAsync(id);
            if (leuge == null)
            {
                return HttpNotFound();
            }
            return View(leuge);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            leuge leuge = await db.leuges.FindAsync(id);
            db.leuges.Remove(leuge);
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
