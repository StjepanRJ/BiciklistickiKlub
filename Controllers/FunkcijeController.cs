using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Security;
using BiciklistickiKlub.Misc;
using BiciklistickiKlub.Models;

namespace BiciklistickiKlub.Controllers
{
    [Authorize(Roles = OvlastiKorisnik.Administrator)]
    public class FunkcijeController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        // GET: Funkcije
        public ActionResult Index()
        {
            return View(db.PopisFunkcija.ToList());
        }

        // GET: Funkcije/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funkcija funkcije = db.PopisFunkcija.Find(id);
            if (funkcije == null)
            {
                return HttpNotFound();
            }
            return View(funkcije);
        }

        // GET: Funkcije/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Funkcije/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sifra,Naziv,Aktivnost")] Funkcija funkcije)
        {
            if (ModelState.IsValid)
            {
                db.PopisFunkcija.Add(funkcije);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funkcije);
        }

        // GET: Funkcije/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funkcija funkcije = db.PopisFunkcija.Find(id);
            if (funkcije == null)
            {
                return HttpNotFound();
            }
            return View(funkcije);
        }

        // POST: Funkcije/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sifra,Naziv,Aktivnost")] Funkcija funkcije)
        {
            if (ModelState.IsValid)
            {
                db.Entry(funkcije).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(funkcije);
        }

        // GET: Funkcije/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funkcija funkcije = db.PopisFunkcija.Find(id);
            if (funkcije == null)
            {
                return HttpNotFound();
            }
            return View(funkcije);
        }

        // POST: Funkcije/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Funkcija funkcije = db.PopisFunkcija.Find(id);

            var clanovi = db.PopisClanova.FirstOrDefault(x => x.SifraFunkcije == id);

            if (clanovi == null) {
                db.PopisFunkcija.Remove(funkcije);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //return View(funkcije);
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
