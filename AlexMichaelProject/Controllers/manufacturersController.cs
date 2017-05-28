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
    public class manufacturersController : Controller
    {
        private DBEntities db = new DBEntities();

        public async Task<ActionResult> Index()
        {
            return View(await db.manufacturers.ToListAsync());
        }

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            manufacturer manufacturer = await db.manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "mname,msite")] manufacturer manufacturer)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                manufacturer dbMan = await db.manufacturers.FindAsync(manufacturer.mname);
                if(dbMan == null)
                {
                    db.manufacturers.Add(manufacturer);
                    await db.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    result = "true";
                }
                else
                {
                    result = "false";
                }

            }
            //return View(manufacturer);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            manufacturer manufacturer = await db.manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "mname,msite")] manufacturer manufacturer)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                db.Entry(manufacturer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";
            }
            //return View(manufacturer);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            manufacturer manufacturer = await db.manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            string result = "";
            try
            {
                manufacturer manufacturer = await db.manufacturers.FindAsync(id);
                db.manufacturers.Remove(manufacturer);
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";
            }
            catch(Exception ex)
            {
                result = "false";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
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
