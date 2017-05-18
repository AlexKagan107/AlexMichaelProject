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
    public class usersController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }
        // GET: users
        public async Task<ActionResult> Index()
        {
            return View(await db.users.ToListAsync());
        }

        // GET: users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = await db.users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.usercreditcards = db.paymentoptions.FindAsync(id);
            return View(user);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            ViewBag.favoriteClub = new SelectList(db.teams, "teamName", "teamName");
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "username,password,fName,fLname,adress,phoneNumber,mail,favoriteClub,isadmin")] user user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                await db.SaveChangesAsync();
                await AddClubToUser2(user.username, user.favoriteClub);
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            ViewBag.favoriteClub = new SelectList(db.teams, "teamName", "teamName");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = await db.users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "username,password,fName,fLname,adress,phoneNumber,mail,favoriteClub,isadmin")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = await db.users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    user user = await db.users.FindAsync(id);
        //    db.users.Remove(user);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            user users = await db.users.FindAsync(id);
            team[] clubArray = users.teams.ToArray<team>();
            for (int i = 0; i < clubArray.Length; i++)
            {
                users.teams.Remove(clubArray[i]);
            }
            db.users.Remove(users);

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

        public ActionResult AddClubToUser()
        {
            ViewBag.teamName = new SelectList(db.teams, "teamName", "teamName");
            ViewBag.username = new SelectList(db.users, "username", "username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddClubToUser([Bind(Include = "username,teamName")] FansOfClubs model)
        {
            if (ModelState.IsValid)
            {
                team club = await db.teams.FindAsync(model.teamName);
                user user = await db.users.FindAsync(model.username);
                user.teams.Add(club);
                await db.SaveChangesAsync();
                return RedirectToAction("AddClubToUser");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddClubToUser2(string username, string teamName)
        {
            if (ModelState.IsValid)
            {
                team club = await db.teams.FindAsync(teamName);
                user user = await db.users.FindAsync(username);
                user.teams.Add(club);
                await db.SaveChangesAsync();
                return RedirectToAction("AddClubToUser");
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> loginUser(user model)
        {

            user user = await db.users.SingleOrDefaultAsync(x => x.username == model.username && x.password == model.password);
            string result = "fail";
            if (user != null)
            {
                Session["username"] = user.username;
                Session["fullname"] = user.fName + " " + user.fLname;
                Session["power"] = user.isadmin;
                if (user.isadmin.Equals("admin"))
                {
                    result = "admin";
                }
                else { result = "user"; }
                
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }



        

        public ActionResult AddCardToUser()
        {
            ViewBag.username = new SelectList(db.users, "username", "username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> addCardToUser([Bind(Include = "username,cardnumber")] CardBelongsTo model)
        {
            if (ModelState.IsValid)
            {
                paymentoption card = await db.paymentoptions.FindAsync(model.cardnumber);
                
                user user = await db.users.FindAsync(model.username);
                user.paymentoptions.Add(card);
                await db.SaveChangesAsync();
                return RedirectToAction("AddCardToUser");
            }
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> addCardToUser2(string username, string teamName)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        team club = await db.teams.FindAsync(teamName);
        //        user user = await db.users.FindAsync(username);
        //        user.teams.Add(club);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("AddClubToUser");
        //    }
        //    return View();
        //}
    }
}
