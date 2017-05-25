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
    public class productsController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            var products = db.products.Include(p => p.manufacturer1).Include(p => p.team);
            return View(await products.ToListAsync());
        }

        [HttpPost]
        public async Task<PartialViewResult> Index(string clubName)
        {
            var products = db.products.Where(o=>o.clubName.StartsWith(clubName)).Include(p => p.manufacturer1).Include(p => p.team);
            return PartialView(await products.ToListAsync());
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "mname");
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "product1,type,manufacturer,clubName,cost,size,description,photo")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "msite", product.manufacturer);
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName", product.clubName);
            return View(product);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "msite", product.manufacturer);
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName", product.clubName);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "product1,type,manufacturer,clubName,cost,size,description,photo")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "msite", product.manufacturer);
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName", product.clubName);
            return View(product);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = await db.products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            product product = await db.products.FindAsync(id);
            db.products.Remove(product);
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

        public async Task<ActionResult> addToCart(string id)
        {
            String username = (String)Session["username"];
            List<product> cart;
            int counter = 0;
            if (Session["cart"] == null)
            {
                cart = new List<product>();
                counter = 0;
            }
            else
            {
                cart = (List<product>)Session["cart"];
                counter = cart.Count;
            }
            product temp = await db.products.FindAsync(id);
            if (temp != null)
            {
                cart.Add(temp);
                counter++;
                Session.Add("cart", cart);
                Session.Add("cartsize", counter);
            }
            return RedirectToAction("showcart","users");
        }

        [HttpPost]
        public async Task<JsonResult> removeFromCart(string id)
        {
            String username = (String)Session["username"];
            
            List<product>cart = (List<product>)Session["cart"];
            
            product temp = await db.products.FindAsync(id);
            List<product> tempList = new List<product>();

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart.ElementAt(i).product1.Equals(temp.product1))
                {
                    cart.RemoveAt(i);
                    int size = (int)Session["cartsize"];
                    size=size-1;
                    Session["cartsize"] = size;
                    break;
                }
                    
            }
            Session["card"] = cart;
            string result = "true";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
