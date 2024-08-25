using BiciklistickiKlub.Misc;
using BiciklistickiKlub.Models;
using BiciklistickiKlub.Reports;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    [Authorize]
    public class ClanoviController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();

        // GET: Clanovi
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Početna o članovima";
            ViewBag.Klub = "Biciklistički klub MEV";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Popis()
        {


            var funkcijeList = bazaPodataka.PopisFunkcija.OrderBy(x => x.Naziv).ToList();
            ViewBag.Funkcije = funkcijeList;


            return View();
        }

        [AllowAnonymous]
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
        {/*
            if(c.Oib.Length != 11 )
            {
                ModelState.AddModelError("Oib", "Broj znamenki nije validan. Potrebno je 11 !");
            }
            else 
            {
                if (!OIB.CheckOIB(c.Oib))
                {
                    ModelState.AddModelError("Oib", "Neispravan OIB");
                }
            }
               */

            if (!OIB.CheckOIB(c.Oib))
            {
                ModelState.AddModelError("Oib", "Neispravan OIB");
            }

            if (c.KategorijaClanstva == 0)
            {
                ModelState.AddModelError("KategorijaClanstva", "Obavezno popuniti");
            }
            if(c.KategorijaClana == 0)
            {
                ModelState.AddModelError("KategorijaClana", "Obavezno popunitit");
            }

            var clanPr = bazaPodataka.PopisClanova.FirstOrDefault(x => x.SifraFunkcije == "PR");
            var clanDpr = bazaPodataka.PopisClanova.FirstOrDefault(x => x.SifraFunkcije == "DPR");
            var existingEntity = bazaPodataka.PopisClanova.Find(c.Id);

            // Provera da li je trenutni korisnik predsjednik

            // Provera da li je trenutni korisnik predsjednik
            var isPredsjednik = existingEntity != null && existingEntity.SifraFunkcije == "PR";

            // Provera da li se pokušava promeniti uloga u predsjednika
            var tryingToBePredsjednik = c.SifraFunkcije == "PR";

            // Ako postoji samo jedan predsjednik i trenutni korisnik je taj predsjednik,
            // omogući ažuriranje podataka bez brisanja ili ponovnog dodavanja u bazu
            if (clanPr != null && isPredsjednik && !tryingToBePredsjednik)
            {
                if (existingEntity != null)
                {
                    // Ažuriraj entitet u kontekstu
                    bazaPodataka.Entry(existingEntity).State = EntityState.Modified;
                }
            }

            if (clanPr != null && c.SifraFunkcije == clanPr.SifraFunkcije && c.Id != clanPr.Id)
            {
                ModelState.AddModelError("SifraFunkcije", "Ne može biti dva predsjednika");
            }
            else if (clanPr != null && c.SifraFunkcije == clanDpr.SifraFunkcije && c.Id != clanDpr.Id)
            {
                ModelState.AddModelError("SifraFunkcije", "Ne može biti dva dopredsjednika");
            }
           


            if (ModelState.IsValid)
            {
                if (existingEntity != null)
                {
                    

                    // Ažuriraj entitet u kontekstu
                    bazaPodataka.Entry(existingEntity).CurrentValues.SetValues(c);
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

        [AllowAnonymous]
        public ActionResult Galerija()
        {
            return View();
        }

        public ActionResult IspisClanova(string naziv, string spol,string funkcija, string sort)
        {
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

            ClanoviReport clanoviReport = new ClanoviReport();
            clanoviReport.ListaClanova(clanovi);

            return File(clanoviReport.Podaci, System.Net.Mime.MediaTypeNames.Application.Pdf, "PopisClanova.pdf");

        }


    }
}