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
    public class purchasesController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: purchases
        public async Task<ActionResult> Index()
        {
            var purchases = db.purchases.Include(p => p.paymentoption).Include(p => p.product1).Include(p => p.user);
            return View(await purchases.ToListAsync());
        }

        // GET: purchases/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase purchase = await db.purchases.FindAsync(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // GET: purchases/Create
        public ActionResult Create()
        {
            ViewBag.creditcardnumber = new SelectList(db.paymentoptions, "creditcardnumber", "creditcardcompany");
            ViewBag.product = new SelectList(db.products, "product1", "type");
            ViewBag.username = new SelectList(db.users, "username", "password");
            return View();
        }

        // POST: purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "username,product,creditcardnumber,dateOfPurchase,dealID")] purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.purchases.Add(purchase);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.creditcardnumber = new SelectList(db.paymentoptions, "creditcardnumber", "creditcardcompany", purchase.creditcardnumber);
            ViewBag.product = new SelectList(db.products, "product1", "type", purchase.product);
            ViewBag.username = new SelectList(db.users, "username", "password", purchase.username);
            return View(purchase);
        }

        


        // GET: purchases/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase purchase = await db.purchases.FindAsync(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.creditcardnumber = new SelectList(db.paymentoptions, "creditcardnumber", "creditcardcompany", purchase.creditcardnumber);
            ViewBag.product = new SelectList(db.products, "product1", "type", purchase.product);
            ViewBag.username = new SelectList(db.users, "username", "password", purchase.username);
            return View(purchase);
        }

        // POST: purchases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "username,product,creditcardnumber,dateOfPurchase,dealID")] purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.creditcardnumber = new SelectList(db.paymentoptions, "creditcardnumber", "creditcardcompany", purchase.creditcardnumber);
            ViewBag.product = new SelectList(db.products, "product1", "type", purchase.product);
            ViewBag.username = new SelectList(db.users, "username", "password", purchase.username);
            return View(purchase);
        }

        // GET: purchases/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            purchase purchase = await db.purchases.FindAsync(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            purchase purchase = await db.purchases.FindAsync(id);
            db.purchases.Remove(purchase);
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
