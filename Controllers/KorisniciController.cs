using BiciklistickiKlub.Misc;
using BiciklistickiKlub.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BiciklistickiKlub.Controllers
{
    [Authorize(Roles = OvlastiKorisnik.Administrator)]
    public class KorisniciController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        LogiranKorisnik logi;
        // GET: Korisnici
        public ActionResult Index()
        {
            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();
            
            return View(listaKorisnika);
        }

        [HttpGet,Authorize]
        public ActionResult PromjenaLozinke()
        {

            return View(); 

        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult PromjenaLozinke(PromjenaLozinke pl)
        {
            if (ModelState.IsValid)
            {
                LogiranKorisnik k = new LogiranKorisnik();
                
                var loz = Misc.PasswordHelper.IzracunajHash(pl.StaraLozinka);

                var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.Lozinka == loz);

                korisnik.Lozinka = Misc.PasswordHelper.IzracunajHash(pl.NovaLozinka);

                korisnik.Ime = korisnik.Ime;
                korisnik.SifraOvlasti = korisnik.SifraOvlasti;
                korisnik.KorisnickoIme = korisnik.KorisnickoIme;
                korisnik.Prezime = korisnik.Prezime;
                korisnik.Email = korisnik.Email;
                korisnik.Aktivan = korisnik.Aktivan;
                korisnik.LozinkaUnos = "1";
                korisnik.LozinkaUnos2 = "1";

                bazaPodataka.Entry(korisnik).State = System.Data.Entity.EntityState.Modified;

                bazaPodataka.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();

        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult Prijava(string returnUrl)
        {
            KorisnikPrijava model = new KorisnikPrijava();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Prijava(KorisnikPrijava model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var korisnikBaza = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == model.KorisnickoIme);

                if (korisnikBaza.Aktivan  )
                {
                    if (korisnikBaza != null)
                    {
                        var passwordOK = korisnikBaza.Lozinka == Misc.PasswordHelper.IzracunajHash(model.Lozinka);

                        if (passwordOK)
                        {
                            logi = new LogiranKorisnik(korisnikBaza);
                            LogiranKorisnik prijavljeniKorisnik = new LogiranKorisnik(korisnikBaza);
                            LogiranKorisnikSerializedModel serializeModel = new LogiranKorisnikSerializedModel();
                            serializeModel.CopyFromUser(prijavljeniKorisnik);
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            string korisnickiPodaci = serializer.Serialize(serializeModel);

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                prijavljeniKorisnik.Identity.Name,
                                DateTime.Now,
                                DateTime.Now.AddHours(1),
                                false,
                                korisnickiPodaci);

                            string ticketEncrypted = FormsAuthentication.Encrypt(authTicket);

                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncrypted);
                            Response.Cookies.Add(cookie);


                            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nemate pravo aktivnosti!");
                    return View(model);
                }
            }

            ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka");
            return View(model);
        }

        [OverrideAuthorization]
        [Authorize]
        public ActionResult Odjava()
        {
            
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Azuriraj(string email)
        {
            
            if (String.IsNullOrEmpty(email))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Korisnik  korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.Email == email);
           
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            return View(korisnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Azuriraj( Korisnik k)
        {
            
            if (!ModelState.IsValid)
            {
                k.LozinkaUnos = "1";
                k.LozinkaUnos2 = "1";
                if(k.Aktivan == true)
                {
                    await ProvjeriIPosaljiObavijesti(k);
                }
                bazaPodataka.Entry(k).State = System.Data.Entity.EntityState.Modified;
                
                bazaPodataka.SaveChanges();

                return RedirectToAction("Index");
            }
                return View(k);
        }


        
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registracija(Korisnik model)
        {
            if (!String.IsNullOrWhiteSpace(model.KorisnickoIme))
            {

                var korImeZauzeto = bazaPodataka.PopisKorisnika.Any(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korImeZauzeto)
                {
                    ModelState.AddModelError("KorisnickoIme", "Korisničko ime je već zauzeto");
                }
            }
            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var emailZauzet = bazaPodataka.PopisKorisnika.Any(x => x.Email == model.Email);
                if (emailZauzet)
                {
                    ModelState.AddModelError("Email", "Email je već zauzet");
                }
            }
            

            if (ModelState.IsValid)
            {
                model.Lozinka = Misc.PasswordHelper.IzracunajHash(model.LozinkaUnos);
                model.SifraOvlasti = "MO";
                model.Aktivan = false;

                bazaPodataka.PopisKorisnika.Add(model);
                bazaPodataka.SaveChanges();

                return View("RegistracijaOK");
            }

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        private string GetKorisnikovEmail(string korisnikId)
        {
            var korisnik = bazaPodataka.PopisKorisnika.FirstOrDefault(u => u.KorisnickoIme == korisnikId);
            if (korisnik != null)
            {
                return korisnik.Email;
            }
            return "Nema email adrese za korisnika";
        }

        // Funkcija za slanje maila korisniku ako se danasnji datum poklapa sa datumom neke aktivnosti
        private async Task ProvjeriIPosaljiObavijesti(Korisnik k)
        {
            DateTime currentDate = DateTime.Now.Date;
           

          
            
                
                        string toEmail = k.Email;
                        string subject = $"Podsjetnik: Planirana aktivnost - ";
                        string body = $"Poštovani ,<br />" +
              $"Želimo vas podsjetiti na nadolazeću aktivnost koja je planirana za .<br /> Detalji aktivnosti su sljedeći:<br />" +
              $"Naziv aktivnosti: <br />" +
              $"Vrsta aktivnosti: <br />" +
              $"Srdačan pozdrav";


                        string apiKey = "7B5L2GFJ1D51USS2PSFMD18P";
                        await SendEmail(apiKey, toEmail, subject, body);
                        /*
                        // Ažurirajte datum poslednje obavijesti
                        aktivnost.PoslednjaObavijest = DateTime.Now;
                        db.Entry(aktivnost).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        */
                    
                
            
        }


        private async Task SendEmail(string apiKey, string toEmail, string subject, string body)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("s77074207@gmail.com", "Activity Tracker");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

        }

        public ActionResult Lijecnicki()
        {


            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();

            return View(listaKorisnika);
        }
    }
}