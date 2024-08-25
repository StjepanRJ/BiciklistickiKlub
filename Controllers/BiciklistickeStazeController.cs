using BiciklistickiKlub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class BiciklistickeStazeController : Controller
    {
         BazaDbContext db = new BazaDbContext();

        // GET: BikeRoutes
        public ActionResult Index()
        {
            var routes = db.PopisBiciklistickihStaza.ToList();
            return View(routes);
        }

        // GET: BikeRoutes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BikeRoutes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name, string geoJson)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var route = new BiciklistickaStaza
                {
                    Ime = name,
                    GeoJson = geoJson,
                    Datum = DateTime.Now,
                    UserId = userId
                };

                db.PopisBiciklistickihStaza.Add(route);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: BikeRoutes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var route = db.PopisBiciklistickihStaza.FirstOrDefault(r => r.Id == id);
            if (route == null)
            {
                return HttpNotFound();
            }

            return View(route);
        }

        // Dispose method for releasing resources
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
