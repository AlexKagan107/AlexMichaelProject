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
    public class paymentoptionsController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            return View(await db.paymentoptions.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            paymentoption paymentoption = await db.paymentoptions.FindAsync(id);
            if (paymentoption == null)
            {
                return HttpNotFound();
            }
            return View(paymentoption);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "creditcardnumber,creditcardcompany,expireddate")] paymentoption paymentoption)
        {
            if (ModelState.IsValid)
            {
                db.paymentoptions.Add(paymentoption);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(paymentoption);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            paymentoption paymentoption = await db.paymentoptions.FindAsync(id);
            if (paymentoption == null)
            {
                return HttpNotFound();
            }
            return View(paymentoption);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "creditcardnumber,creditcardcompany,expireddate")] paymentoption paymentoption)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paymentoption).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(paymentoption);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            paymentoption paymentoption = await db.paymentoptions.FindAsync(id);
            if (paymentoption == null)
            {
                return HttpNotFound();
            }
            return View(paymentoption);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            paymentoption paymentoption = await db.paymentoptions.FindAsync(id);
            db.paymentoptions.Remove(paymentoption);
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
