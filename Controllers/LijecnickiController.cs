using BiciklistickiKlub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class LijecnickiController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        // GET: Liječnićki
        public ActionResult Index()
        {
            return View(bazaPodataka.PopisLijecnickih.ToList());
        }

        public ActionResult Azuriraj(int? id)
        {
            Lijecnicki lije = null;
            if (!id.HasValue)
            {
                lije = new Lijecnicki();
                ViewBag.Title = "Kreiranje liječničkog";
                ViewBag.Novi = true;
            }
            else
            {

                lije = bazaPodataka.PopisLijecnickih.FirstOrDefault(x => x.Id == id);

                if (lije == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Title = "Ažuriranje podataka o liječnićkom";
                ViewBag.Novi = false;

            }

            var funkcije = bazaPodataka.PopisKorisnika.OrderBy(x => x.KorisnickoIme).ToList();
            funkcije.Insert(0, new Korisnik { KorisnickoIme = "", Ime = "Nedefinirano" });
            ViewBag.Funkcije = funkcije;

            return View(lije);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Azuriraj(Lijecnicki l)
        {
            


            if (ModelState.IsValid)
            {
                if (l.Id != 0)
                {
                    bazaPodataka.Entry(l).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    bazaPodataka.PopisLijecnickih.Add(l);
                }
                bazaPodataka.SaveChanges();

                return RedirectToAction("Index");
            }
            if (l.Id == 0)
            {
                ViewBag.Title = "Kreiranje clana";
                ViewBag.Novi = true;
            }
            else
            {
                ViewBag.Title = "Ažuriranje podataka o članu";
                ViewBag.Novi = false;
            }

            var funkcije = bazaPodataka.PopisKorisnika.OrderBy(x => x.KorisnickoIme).ToList();
            funkcije.Insert(0, new Korisnik { KorisnickoIme = "", Ime = "Nedefinirano" });
            ViewBag.Funkcije = funkcije;
            return View(l);
        }

        public ActionResult Lijecnicki()
        {


            var listaKorisnika = bazaPodataka.PopisLijecnickih.OrderBy(x => x.KorisnickoIme).ThenBy(x => x.KorisnickoIme).ToList();

            return View(listaKorisnika);
        }
    }
}