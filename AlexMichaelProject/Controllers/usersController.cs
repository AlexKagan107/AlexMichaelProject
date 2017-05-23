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

        public async Task<ActionResult> BuyCart()
        {
            string username = (string)Session["username"];
            user tempuser = await db.users.FindAsync(username);
            ViewBag.paymentoptions = new SelectList(tempuser.paymentoptions, "creditcardnumber", "creditcardnumber");

            return View();
        }

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }
        // GET: users
        public async Task<ActionResult> Index()
        {
            return View(await db.users.ToListAsync());
        }
        public async Task<ActionResult> showcart()
        {
            return View(await db.products.ToListAsync());
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
            userCreditCards(id);
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
            string result = "true";
            if (ModelState.IsValid)
            {
                user dbUser = await db.users.FindAsync(user.username);
                if (dbUser == null)
                {
                    db.users.Add(user);
                    await db.SaveChangesAsync();
                    await AddClubToUser2(user.username, user.favoriteClub);
                    result = "true";
                }
                else
                {
                    result = "false";
                }
            }
            //return View(user);
            return Json(result, JsonRequestBehavior.AllowGet);
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
            string result = "true";
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                result = "true";
                //return RedirectToAction("Index");
            }
            //return View(user);
            return Json(result, JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            string result = "true";
            try
            {
                user users = await db.users.FindAsync(id);
                team[] clubArray = users.teams.ToArray<team>();
                for (int i = 0; i < clubArray.Length; i++)
                {
                    users.teams.Remove(clubArray[i]);
                }
                db.users.Remove(users);
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
            string result = "";
            if (ModelState.IsValid)
            {
                team club = await db.teams.FindAsync(model.teamName);
                user user = await db.users.FindAsync(model.username);
                user.teams.Add(club);
                await db.SaveChangesAsync();
                result = "true";
                //return RedirectToAction("AddClubToUser");
            }
            //return View();
            return Json(result, JsonRequestBehavior.AllowGet);
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
                Session["fanclub"] = user.favoriteClub;
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
            string result = "";
            if (ModelState.IsValid)
            {
                paymentoption card = await db.paymentoptions.FindAsync(model.cardnumber);
                if (card != null)
                {
                    user user = await db.users.FindAsync(model.username);
                    user.paymentoptions.Add(card);
                    await db.SaveChangesAsync();
                    result = "true";
                }
                else
                {
                    result = "false";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public async void userCreditCards(String username)
        {
            user users = await db.users.FindAsync(username);
            paymentoption[] creditCards = users.paymentoptions.ToArray();
            ViewBag.userCreditCards = creditCards;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyCart(int paymentoptions)
        {
            if (ModelState.IsValid)
            {
                List<product> cartItems = (List<AlexMichaelProject.Models.product>)Session["cart"];
                string username = (string)Session["username"];
                for (int i = 0; i < cartItems.Count; i++)
                {
                    int j = 0;
                    purchase temp = new purchase();
                    temp.username = username;
                    temp.product = cartItems.ElementAt(i).product1;
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
                }
                Session["cart"] = null;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "products");
            }
            return View();
        }


    }
}
