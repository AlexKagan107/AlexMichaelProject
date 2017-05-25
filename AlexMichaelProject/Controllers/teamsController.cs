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
    public class teamsController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            return View(await db.teams.ToListAsync());
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            team team = await db.teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        public ActionResult Create()
        {
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "teamName,leuge,homeStadium")] team team)
        {
            string result = "true";
            if (ModelState.IsValid)
            {
                team dbTeam = await db.teams.FindAsync(team.teamName);
                if (dbTeam == null)
                {
                    db.teams.Add(team);
                    await db.SaveChangesAsync();
                    result = "true";
                }
                else
                {
                    result = "false";
                }
            }
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            team team = await db.teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "teamName,leuge,homeStadium")] team team)
        {
            string result = "true";
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                await db.SaveChangesAsync();
                result = "true";
            }
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            team team = await db.teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            string result = "true";
            try
            {
                team team = await db.teams.FindAsync(id);
                db.teams.Remove(team);
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
