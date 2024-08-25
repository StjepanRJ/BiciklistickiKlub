using BiciklistickiKlub.Models;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Asn1.Mozilla;
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
        public ActionResult Azuriraj1(int? id)
        {
            Lijecnicki lije = bazaPodataka.PopisLijecnickih.FirstOrDefault(x => x.Id == id);

            var razlika = (((uint)DateTime.Now.Month) - ((uint)lije.DatumLijecnickog.Month));
            ViewBag.Razlika = razlika;
            /*if(razlika >= 0)
            {
                lije.Obavljen = true;
            }
            */
            if (lije == null)
            {
                return HttpNotFound();
            }
            /*
            if(lije.Obavljen == true)
            {
                return RedirectToAction("Index");
            }
            */

            ViewBag.Title = "Ažuriranje podataka o liječnićkom";
            ViewBag.Novi = false;

            var funkcije = bazaPodataka.PopisKorisnika.OrderBy(x => x.KorisnickoIme).ToList();
            funkcije.Insert(0, new Korisnik { KorisnickoIme = "", Ime = "Nedefinirano" });
            ViewBag.Funkcije = funkcije;

            return View(lije);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Azuriraj1(Lijecnicki l)
        {

            bool F = false;
            var popis = bazaPodataka.PopisLijecnickih.ToList();
            foreach (Lijecnicki li in popis)
            {
                if ((li.Obavljen == false) && (li.KorisnickoIme == l.KorisnickoIme) && (li.Id != l.Id))
                {
                    F = true;
                }
            }
            if (F)
            {
                ModelState.AddModelError("KorisnickoIme", "Ovaj korisnik već ima zakazan liječnićki pregled");
            }
            /*
            if (l.DatumLijecnickog < DateTime.Now)
            {
                ModelState.AddModelError("DatumLijecnickog", "Datum nemoguć");
            }
            */
            var razlika = (((uint)DateTime.Now.Month) - ((uint)l.DatumLijecnickog.Month));
            ViewBag.Razlika = razlika;
            /*
            if ((((uint)DateTime.Now.Month) - ((uint)l.DatumLijecnickog.Month)) > 1)
            {
                ModelState.AddModelError("DatumLijecnickog", "Datum nemoguć");
               
            }
            */
            var existingEntity = bazaPodataka.PopisLijecnickih.Find(l.Id);

            if (existingEntity != null && existingEntity.KorisnickoIme != l.KorisnickoIme)
            {
                ModelState.AddModelError("KorisnickoIme", "Korisničko ime ne može biti promjenjeno.");
            }
            /*
            if (l.DatumLijecnickog < DateTime.Now)
            {
                ModelState.AddModelError("DatumLijecnickog", "Datum nemoguć");
            }
            */
            if (ModelState.IsValid)
            {
               
                if (existingEntity != null)
                {
                    /*
                    if(razlika > 2)
                    {
                        l.DatumLijecnickog = existingEntity.DatumLijecnickog;
                        
                    }
                    */
                    // Zadrži originalno korisničko ime
                    l.KorisnickoIme = existingEntity.KorisnickoIme;
                    // Entitet već postoji u kontekstu, ažuriraj ga
                    bazaPodataka.Entry(existingEntity).CurrentValues.SetValues(l);
                }
                else
                {
                    // Entitet je novi, dodaj ga u kontekst
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
                /*
                if(lije.Obavljen == true)
                {
                    return RedirectToAction("Index");
                }
                */

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
            Lijecnicki daliPostojiF = new Lijecnicki();
            Lijecnicki daliPostojiT = new Lijecnicki();
            bool F = false;
            
            var popis = bazaPodataka.PopisLijecnickih.ToList();
            foreach(Lijecnicki li in popis)
            {
                if((li.Obavljen == false) && (li.KorisnickoIme == l.KorisnickoIme))
                {
                    F = true;
                }
            }
          
           
            var daliPostoji2 = bazaPodataka.PopisLijecnickih.Any(x => x.KorisnickoIme == l.KorisnickoIme);
           
                if ((l.KorisnickoIme != null) && (daliPostoji2 == true))
                {

                    if ((F))
                    {
                        ModelState.AddModelError("KorisnickoIme", "Postoji zakazan liječnićki sa ovim imenom");
                    }
                }
            
            if (l.DatumLijecnickog < DateTime.Now)
            {
                ModelState.AddModelError("DatumLijecnickog", "Datum nemoguć");
            }
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