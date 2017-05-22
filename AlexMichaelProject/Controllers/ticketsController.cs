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
    public class ticketsController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: tickets
        public async Task<ActionResult> Index()
        {
            var tickets = db.tickets.Include(t => t.match);
            return View(await tickets.ToListAsync());
        }

        // GET: tickets/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = await db.tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: tickets/Create
        public ActionResult Create()
        {
            ViewBag.matchID = new SelectList(db.matches, "matchID", "matchID");
            
            return View();
        }

        // POST: tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "ticketID,matchID,seatNumber,cost,seatType")] ticket ticket)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.tickets.Add(ticket);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.matchID = new SelectList(db.matches, "matchID", "teamA", ticket.matchID);
        //    return View(ticket);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int matchID, int seatNumber,int cost,string seatType,string amount)
        {
            if (ModelState.IsValid)
            {
                int quanity = Int32.Parse(amount);
                int[] ticketsID = new int[quanity];
                int counter = 0;
                for(int i=0; counter < quanity; i++)
                {
                    ticket temp = new ticket();
                    temp = await db.tickets.FindAsync(i);
                    if (temp == null)
                    {
                        ticketsID[counter] = i;
                        counter++;
                    }
                    
                }
                match tempMatch = await db.matches.FindAsync(matchID);
                
                
                for(int j=0;j< quanity; j++)
                {
                    ticket ticket = new ticket();
                    ticket.matchID = tempMatch.matchID;
                    ticket.cost = cost;
                    ticket.seatType = seatType;
                    int seatNumberIndex = seatNumber;
                    ticket.seatNumber = seatNumberIndex;
                    seatNumberIndex++;
                    ticket.ticketID = ticketsID[j];
                    db.tickets.Add(ticket);
                    await db.SaveChangesAsync();

                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");

        }

        // GET: tickets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = await db.tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.matchID = new SelectList(db.matches, "matchID", "teamA", ticket.matchID);
            return View(ticket);
        }

        // POST: tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ticketID,matchID,seatNumber,cost,seatType")] ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.matchID = new SelectList(db.matches, "matchID", "teamA", ticket.matchID);
            return View(ticket);
        }

        // GET: tickets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ticket ticket = await db.tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ticket ticket = await db.tickets.FindAsync(id);
            db.tickets.Remove(ticket);
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
