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
    public class matchesController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            var matches = db.matches.Include(m => m.team).Include(m => m.team1);
            return View(await matches.ToListAsync());
        }

        [HttpPost]
        public async Task<PartialViewResult> Index(string type, string text)
        {
            if(type == "teamA")
            { 
                var matches = db.matches.Where(o=>o.teamA.StartsWith(text)).Include(m => m.team).Include(m => m.team1);
                return PartialView(await matches.ToListAsync());
            }
            if (type == "teamB")
            {
                var matches = db.matches.Where(o => o.teamB.StartsWith(text)).Include(m => m.team).Include(m => m.team1);
                return PartialView(await matches.ToListAsync());
            }
            if (type == "stadium")
            {
                var matches = db.matches.Where(o => o.stadium.StartsWith(text)).Include(m => m.team).Include(m => m.team1);
                return PartialView(await matches.ToListAsync());
            }
            if (type == "league")
            {
                var matches = db.matches.Where(o => o.league.StartsWith(text)).Include(m => m.team).Include(m => m.team1);
                return PartialView(await matches.ToListAsync());
            }
            return PartialView();

        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            match match = await db.matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        public ActionResult Create()
        {
            for(int i = 0; i < db.matches.ToList().Count; i++)
            {
                if (db.matches.Find(i) == null)
                {
                    Session["matchid"] = i;
                    break;
                }
            }
            ViewBag.teamA = new SelectList(db.teams, "teamName", "teamName");
            ViewBag.teamB = new SelectList(db.teams, "teamName", "teamName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "matchID,teamA,teamB,stadium,league,date")] match match)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                match dbMatch = await db.matches.FindAsync(match.matchID);
                if(dbMatch == null)
                {
                    db.matches.Add(match);
                    await db.SaveChangesAsync();
                    result = "true";
                }
                else
                {
                    result = "false";
                }

            }

            ViewBag.teamA = new SelectList(db.teams, "teamName", "teamName", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "teamName", match.teamB);
            //return View(match);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            match match = await db.matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.teamA = new SelectList(db.teams, "teamName", "teamName", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "teamName", match.teamB);
            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "matchID,teamA,teamB,stadium,league,date")] match match)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";
            }
            ViewBag.teamA = new SelectList(db.teams, "teamName", "teamName", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "teamName", match.teamB);
            //return View(match);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            match match = await db.matches.FindAsync(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string result = "true";
            try
            {
                match match = await db.matches.FindAsync(id);
                db.matches.Remove(match);
                await db.SaveChangesAsync();
                result = "true";
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
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
