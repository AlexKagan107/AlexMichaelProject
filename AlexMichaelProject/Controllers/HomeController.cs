using AlexMichaelProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AlexMichaelProject.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();

        public async Task<ActionResult> Index()
        {
            return View(await db.matches.ToListAsync());
        }

        public ActionResult SideMenu()
        {
            return PartialView("SideMenu");
        }

    }
}