﻿using System;
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
        public async Task<PartialViewResult> Index(string type, string text)
        {
            if (type == "Club")
            {
                var products = db.products.Where(o => o.clubName.StartsWith(text)).Include(p => p.manufacturer1).Include(p => p.team);
                return PartialView(await products.ToListAsync());
            }

            if (type == "Type")
            {
                var products = db.products.Where(o => o.type.StartsWith(text)).Include(p => p.manufacturer1).Include(p => p.team);
                return PartialView(await products.ToListAsync());
            }

            if (type == "Manufacturer")
            {
                var products = db.products.Where(o => o.manufacturer.StartsWith(text)).Include(p => p.manufacturer1).Include(p => p.team);
                return PartialView(await products.ToListAsync());
            }

            if (type == "Size")
            {
                var products = db.products.Where(o => o.size.ToString().StartsWith(text)).Include(p => p.manufacturer1).Include(p => p.team);
                return PartialView(await products.ToListAsync());
            }
            return PartialView();
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

        public async Task<ActionResult> purchaseProd(string id)
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
            string result = "";
            if (ModelState.IsValid)
            {
                product dbProduct = await db.products.FindAsync(product.product1);
                if(dbProduct == null)
                {
                    db.products.Add(product);
                    await db.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    result = "true";
                }
                else
                {
                    result = "false";
                }

            }

            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "msite", product.manufacturer);
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName", product.clubName);
            //return View(product);
            return Json(result, JsonRequestBehavior.AllowGet);
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
            string result = "";
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";
            }
            ViewBag.manufacturer = new SelectList(db.manufacturers, "mname", "msite", product.manufacturer);
            ViewBag.clubName = new SelectList(db.teams, "teamName", "teamName", product.clubName);
            //return View(product);
            return Json(result, JsonRequestBehavior.AllowGet);
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

        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            string result = "";
            try
            {
                product product = await db.products.FindAsync(id);
                db.products.Remove(product);
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
            int size = 0;
            String username = (String)Session["username"];

            String totalSum = (String)Session["sum"];
            int total = Int32.Parse(totalSum);
            int newTotal = 0;
            List<product>cart = (List<product>)Session["cart"];
            
            product temp = await db.products.FindAsync(id);
            int costItem = temp.cost;

            List<product> tempList = new List<product>();

            for (int i = 0; i < cart.Count; i++)
            {
                if (cart.ElementAt(i).product1.Equals(temp.product1))
                {
                    cart.RemoveAt(i);
                     size = (int)Session["cartsize"];
                    size=size-1;
                    Session["cartsize"] = size;
                    break;
                }
                    
            }
            Session["card"] = cart;
            //string result = "true";
            newTotal = total - costItem;
            string result = newTotal.ToString();
            Session["sum"] = result;

            string newSize = size.ToString();

            var response = new { data = result, data2 = newSize };

            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}
