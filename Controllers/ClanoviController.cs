using BiciklistickiKlub.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class ClanoviController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        // GET: Clanovi
        public ActionResult Index()
        {
            ViewBag.Title = "Početna o članovima";
            ViewBag.Klub = "Biciklistički klub MEV";
            return View();
        }

        public ActionResult Popis()
        {


            var funkcijeList = bazaPodataka.PopisFunkcija.OrderBy(x => x.Naziv).ToList();
            ViewBag.Funkcije = funkcijeList;


            return View();
        }

        public ActionResult PopisPartial(string naziv, string spol, string funkcija, string sort, int? page)
        {
            ViewBag.Sortiranje = sort;
            ViewBag.NazivSort = String.IsNullOrEmpty(sort) ? "naziv_desc" : "";
            ViewBag.FunkcijaSort = sort == "funkcija" ? "funkcija_desc" : "funkcija";
            ViewBag.Funkcija = funkcija;
            ViewBag.Naziv = naziv;
            ViewBag.Spol = spol;

            var clanovi = bazaPodataka.PopisClanova.ToList();

            
            //filtriranje

            if (!String.IsNullOrWhiteSpace(naziv))
                clanovi = clanovi.Where(x => x.PrezimeIme.ToUpper().Contains(naziv.ToUpper())).ToList();

            if (!String.IsNullOrWhiteSpace(spol))
                clanovi = clanovi.Where(x => x.Spol == spol).ToList();

            if (!String.IsNullOrWhiteSpace(funkcija))
                clanovi = clanovi.Where(x => x.SifraFunkcije == funkcija).ToList();

            switch (sort)
            {
                case "naziv_desc":
                    clanovi = clanovi.OrderByDescending(c => c.PrezimeIme).ToList();
                    break;
                case "funkcija":
                    clanovi = clanovi.OrderBy(c => c.SifraFunkcije).ToList();
                    break;
                case ("funkcija_desc"):
                    clanovi = clanovi.OrderByDescending(c => c.SifraFunkcije).ToList();
                    break;
                default:
                    clanovi = clanovi.OrderBy(c => c.PrezimeIme).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            return PartialView("_PartialPopis", clanovi.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detalji(int? id)
        {
            if (!id.HasValue)
            {

                return RedirectToAction("Popis");
            }



            Clan clan = bazaPodataka.PopisClanova.FirstOrDefault(x => x.Id == id);

            if(clan == null)
            {

                return RedirectToAction("Popis");
            }

            return View(clan);
        }

        public ActionResult Azuriraj(int? id)
        {
            Clan clan = null;
            if (!id.HasValue)
            {
                clan = new Clan();
                ViewBag.Title = "Kreiranje člana";
                ViewBag.Novi = true;
            }
            else
            {

                clan = bazaPodataka.PopisClanova.FirstOrDefault(x => x.Id == id);

                if (clan == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Title = "Ažuriranje podataka o članu";
                ViewBag.Novi = false;

            }

            var funkcije = bazaPodataka.PopisFunkcija.OrderBy(x => x.Naziv).ToList();
            funkcije.Insert(0, new Funkcija { Sifra = "", Naziv = "Nedefinirano" });
            ViewBag.Funkcije = funkcije;

            return View(clan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Azuriraj(Clan c)
        {
            if (!OIB.CheckOIB(c.Oib))
            {
                ModelState.AddModelError("Oib", "Neispravan OIB");
            }


            if (ModelState.IsValid)
            {
                if (c.Id != 0)
                {
                    bazaPodataka.Entry(c).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    bazaPodataka.PopisClanova.Add(c);
                }
                bazaPodataka.SaveChanges();

                return RedirectToAction("Popis");
            }
            if(c.Id == 0)
            {
                ViewBag.Title = "Kreiranje clana";
                ViewBag.Novi = true;
            }
            else
            {
                ViewBag.Title = "Ažuriranje podataka o članu";
                ViewBag.Novi = false;
            }

            var funkcije = bazaPodataka.PopisFunkcija.OrderBy(x => x.Naziv).ToList();
            funkcije.Insert(0, new Funkcija { Sifra = "", Naziv = "Nedefinirano" });
            ViewBag.Funkcije = funkcije;
            return View(c);
        }

        //Brisanje clana
        //GET metoda
        //
        public ActionResult Brisi(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Popis");
            }

            Clan c = bazaPodataka.PopisClanova.FirstOrDefault(x => x.Id == id);

            if (c == null)
            {
                return HttpNotFound();
            }

            ViewBag.Title = "Potvrda brisanja člana";
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Brisi(int id)
        {
            Clan c = bazaPodataka.PopisClanova.FirstOrDefault(x => x.Id == id);
            if(c == null)
            {
                return HttpNotFound();

            }

            bazaPodataka.PopisClanova.Remove(c);
            bazaPodataka.SaveChanges();

            return View("BrisiStatus");
        }

        public ActionResult Galerija()
        {
            return View();
        }
    }
}