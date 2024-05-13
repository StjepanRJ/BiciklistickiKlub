using BiciklistickiKlub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class KorisniciController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        // GET: Korisnici
        public ActionResult Index()
        {
            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();
            return View(listaKorisnika);
        }
    }
}