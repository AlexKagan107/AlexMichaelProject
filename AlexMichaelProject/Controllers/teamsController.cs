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

        // GET: teams
        public async Task<ActionResult> Index()
        {
            return View(await db.teams.ToListAsync());
        }

        // GET: teams/Details/5
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

        // GET: teams/Create
        public ActionResult Create()
        {
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            return View();
        }

        // POST: teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
            //return View(team);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: teams/Edit/5
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

        // POST: teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
                //return RedirectToAction("Index");
            }
            ViewBag.leuge = new SelectList(db.leuges, "leugeName", "leugeName");
            //return View(team);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: teams/Delete/5
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

        // POST: teams/Delete/5
        //[HttpPost, ActionName("Delete")]
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
