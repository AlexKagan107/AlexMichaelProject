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
    public class ticketsController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

        public async Task<ActionResult> Index()
        {
            var tickets = db.tickets.Include(t => t.match);

            return View(await tickets.ToListAsync());
        }

        public async Task<ActionResult> Buy(int id)
        {
            match temp = await db.matches.FindAsync(id);
            List<ticket> tickets = temp.tickets.ToList();
            return View(tickets.ToList());
        }

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

        public ActionResult Create()
        {
            ViewBag.matchID = new SelectList(db.matches, "matchID", "matchID");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int matchID, int seatNumber, int cost, string seatType, string amount)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                int quanity = Int32.Parse(amount);
                int[] ticketsID = new int[quanity];
                int counter = 0;
                for (int i = 0; counter < quanity; i++)
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

                int seatNumberIndex = seatNumber;
                for (int j = 0; j < quanity; j++)
                {
                    ticket ticket = new ticket();
                    ticket.matchID = tempMatch.matchID;
                    ticket.cost = cost;
                    ticket.seatType = seatType;

                    ticket.seatNumber = seatNumberIndex;
                    seatNumberIndex++;
                    ticket.ticketID = ticketsID[j];
                    db.tickets.Add(ticket);
                    await db.SaveChangesAsync();

                }
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";
            }
            //return RedirectToAction("Index");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

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
            ViewBag.matchID = new SelectList(db.matches, "matchID", "matchID", ticket.matchID);
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ticketID,matchID,seatNumber,cost,seatType")] ticket ticket)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                result = "true";
            }
            ViewBag.matchID = new SelectList(db.matches, "matchID", "matchID", ticket.matchID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            string result = "true";
            try
            {
                ticket ticket = await db.tickets.FindAsync(id);
                db.tickets.Remove(ticket);
                await db.SaveChangesAsync();
                result = "true";
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> purchaset(int id)
        {
            string username = (string)Session["username"];
            user tempuser = await db.users.FindAsync(username);

            ViewBag.paymentoptions = new SelectList(tempuser.paymentoptions, "creditcardnumber", "creditcardnumber");
            ticket t = await db.tickets.FindAsync(id);
            if (t != null)
            {
                Session["ticketPrice"] = t.cost + "";
                Session["ticketid"] = id;
            }
            else
            {
                Session["ticketPrice"] = 0 + "";
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> buyt(int paymentoptions)
        {
            string result = "";
            if (ModelState.IsValid)
            {
                string username = (string)Session["username"];
                user tempuser = await db.users.FindAsync(username);
                int ticketid = (int)Session["ticketid"];
                ticket t = await db.tickets.FindAsync(ticketid);

                product p = new product();
                p.product1 = "999";
                p.type = t.seatType;
                p.manufacturer = t.ticketID + "";
                p.clubName = "ticket";
                p.cost = t.cost;
                p.size = t.seatNumber;
                p.description = t.matchID + "";
                p.photo = null;

                int j = 0;
                purchase temp = new purchase();
                temp.username = username;
                temp.product = p.product1;
                temp.creditcardnumber = paymentoptions;
                temp.dateOfPurchase = DateTime.Today;
                Boolean flag = false;
                while (flag == false)
                {
                    purchase test = new purchase();
                    test = await db.purchases.FindAsync(j);
                    if (test == null)
                    {
                        flag = true;
                    }
                    else
                        j++;

                }
                temp.dealID = j;
                db.purchases.Add(temp);
                db.tickets.Remove(t);
                await db.SaveChangesAsync();
                //return RedirectToAction("Index");
                result = "true";

            }
            //return View();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


    }
}
