using BiciklistickiKlub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class HomeController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();
        public ActionResult Index()
        {
            var listaKorisnika = bazaPodataka.PopisLijecnickih.OrderBy(x => x.KorisnickoIme).ThenBy(x => x.KorisnickoIme).ToList();
           
                if (User.Identity.IsAuthenticated)
                    return View(listaKorisnika);
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}