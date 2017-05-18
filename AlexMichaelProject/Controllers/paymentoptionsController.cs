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

        // GET: paymentoptions
        public async Task<ActionResult> Index()
        {
            return View(await db.paymentoptions.ToListAsync());
        }

        // GET: paymentoptions/Details/5
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

        // GET: paymentoptions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: paymentoptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: paymentoptions/Edit/5
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

        // POST: paymentoptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: paymentoptions/Delete/5
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

        // POST: paymentoptions/Delete/5
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
