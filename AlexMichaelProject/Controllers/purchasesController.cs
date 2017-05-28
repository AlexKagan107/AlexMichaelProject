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

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            var purchases = db.purchases.Include(p => p.paymentoption).Include(p => p.product1).Include(p => p.user);
            return View(await purchases.ToListAsync());
        }

        [HttpPost]
        public async Task<PartialViewResult> Index(string type, string text)
        {
            if (type == "Username")
            {
                var purchases = db.purchases.Where(o => o.username.StartsWith(text)).Include(p => p.paymentoption).Include(p => p.product1).Include(p => p.user);
                return PartialView(await purchases.ToListAsync());
            }
            if (type == "DealID")
            {
                var purchases = db.purchases.Where(o => o.dealID.ToString().StartsWith(text)).Include(p => p.paymentoption).Include(p => p.product1).Include(p => p.user);
                return PartialView(await purchases.ToListAsync());
            }
            if (type == "ProductID")
            {
                var purchases = db.purchases.Where(o => o.product.ToString().StartsWith(text)).Include(p => p.paymentoption).Include(p => p.product1).Include(p => p.user);
                return PartialView(await purchases.ToListAsync());
            }
            return PartialView();

        }

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

        public ActionResult Create()
        {
            ViewBag.creditcardnumber = new SelectList(db.paymentoptions, "creditcardnumber", "creditcardcompany");
            ViewBag.product = new SelectList(db.products, "product1", "type");
            ViewBag.username = new SelectList(db.users, "username", "password");
            return View();
        }

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
