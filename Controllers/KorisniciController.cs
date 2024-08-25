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
        private readonly EmailService _emailService = new EmailService();

       

        BazaDbContext bazaPodataka = new BazaDbContext();
        LogiranKorisnik logi;
        // GET: Korisnici
        public ActionResult Index(string naziv)
        {

            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();

            if (!String.IsNullOrWhiteSpace(naziv))
                listaKorisnika = listaKorisnika.Where(x => x.PrezimeIme.ToUpper().Contains(naziv.ToUpper())).ToList();

                return View(listaKorisnika);
        }

        [HttpGet,Authorize,AllowAnonymous]
        public ActionResult PromjenaLozinke()
        {

            return View(); 

        }

        [HttpPost, Authorize, ValidateAntiForgeryToken,AllowAnonymous]
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
                if (korisnik.SifraOvlasti == "MO")
                {
                    return RedirectToAction("Index", "Home");

                }
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
        
        public ActionResult Azuriraj( Korisnik k)
        {
            
            if (!ModelState.IsValid)
            {
                k.LozinkaUnos = "1";
                k.LozinkaUnos2 = "1";
              
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
        public async Task<ActionResult> Registracija(Korisnik model)
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
                model.Aktivan = true;
                string subject = "Potvrda registracije";
                string body = $"Poštovani {model.KorisnickoIme},<br/><br/>Hvala što ste se registrirali. Vaš račun je uspješno kreiran.";

                await _emailService.SendEmailAsync(model.Email, subject, body);

                bazaPodataka.PopisKorisnika.Add(model);
                bazaPodataka.SaveChanges();

                return View("RegistracijaOK");
            }

            var ovlasti = bazaPodataka.PopisOvlasti.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }

        

       
    }
}