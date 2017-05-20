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
    public class matchesController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: matches
        public async Task<ActionResult> Index()
        {
            var matches = db.matches.Include(m => m.team).Include(m => m.team1);
            return View(await matches.ToListAsync());
        }

        // GET: matches/Details/5
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

        // GET: matches/Create
        public ActionResult Create()
        {
            ViewBag.teamA = new SelectList(db.teams, "teamName", "leuge");
            ViewBag.teamB = new SelectList(db.teams, "teamName", "leuge");
            return View();
        }

        // POST: matches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "matchID,teamA,teamB,stadium,league,date")] match match)
        {
            if (ModelState.IsValid)
            {
                db.matches.Add(match);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.teamA = new SelectList(db.teams, "teamName", "leuge", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "leuge", match.teamB);
            return View(match);
        }

        // GET: matches/Edit/5
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
            ViewBag.teamA = new SelectList(db.teams, "teamName", "leuge", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "leuge", match.teamB);
            return View(match);
        }

        // POST: matches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "matchID,teamA,teamB,stadium,league,date")] match match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.teamA = new SelectList(db.teams, "teamName", "leuge", match.teamA);
            ViewBag.teamB = new SelectList(db.teams, "teamName", "leuge", match.teamB);
            return View(match);
        }

        // GET: matches/Delete/5
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

        // POST: matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            match match = await db.matches.FindAsync(id);
            db.matches.Remove(match);
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