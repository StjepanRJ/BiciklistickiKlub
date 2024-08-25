using BiciklistickiKlub.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        // Prikaz svih kategorija
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.NijePrijavljen = false;
            if (!User.Identity.IsAuthenticated)
            {
                ViewBag.NijePrijavljen = true;
                
            }
            var kategorije = db.PopisKategorija.ToList();
            return View(kategorije);
        }

        // Prikaz tema u kategoriji
        public ActionResult Teme(int kategorijaId)
        {
            var teme = db.PopisTema.Where(t => t.KategorijaId == kategorijaId).ToList();
            ViewBag.KategorijaId = kategorijaId;
            return View(teme);
        }

        // Prikaz postova u temi
        public ActionResult Postovi(int temaId)
        {
            var postovi = db.PopisPostova.Where(p => p.TemaId == temaId).ToList();
            var tema = db.PopisTema.FirstOrDefault(x => x.Id == temaId);
            ViewBag.TemaId = temaId;
            ViewBag.TemaIme = tema.Naslov;
            return View(postovi);
        }

        // Kreiranje nove teme

        [HttpGet]

        public ActionResult NovaTema(int? kategorijaId)
        {
            var kt = db.PopisKategorija.FirstOrDefault(x => x.Id == kategorijaId);

           
            // Kreirajte novi objekt Tema i postavite KategorijaId
            var tema = new Tema
            {
               // KategorijaId = 1 
            };

            return View(tema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovaTema(Tema tema)
        {
            
            if (ModelState.IsValid)
            {
                db.PopisTema.Add(tema);
                db.SaveChanges();
                return RedirectToAction("Teme", new { kategorijaId = tema.KategorijaId });
            }
            return View(tema);
        }

        // Kreiranje novog posta
        [HttpGet]
        public ActionResult NoviPost(int? temaId)
        {
            Post post = null;
            post = new Post
            {
                KorisnickoIme = User.Identity.Name
            };

            ViewBag.Title = "Kreiranje posta";
            ViewBag.Novi = true;
            ViewBag.KIme = User.Identity.Name;

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NoviPost(Post post)
        {
            if (ModelState.IsValid)
            {
                
                post.KorisnickoIme = User.Identity.Name;
                post.DatumKreiranja = DateTime.Now;
                db.PopisPostova.Add(post);
                db.SaveChanges();
                return RedirectToAction("Postovi", new { temaId = post.TemaId });
            }
            return View(post);
        }
        public ActionResult Odgovor(int? TemaId, int? id)
        {
            var pitanje = db.PopisPostova.FirstOrDefault(x => x.Id == id);

            var post = new Post
            {
                KorisnickoIme = User.Identity.Name,
                Pitanje = pitanje.Sadrzaj,
                KorisnickoImePitanje = pitanje.KorisnickoIme
            };
            return View(post);
        }

        [HttpPost]
        public ActionResult Odgovor(Post post)
        {
            if (ModelState.IsValid)
            {

                post.KorisnickoIme = User.Identity.Name;
                post.DatumKreiranja = DateTime.Now;
                db.PopisPostova.Add(post);
                db.SaveChanges();
                return RedirectToAction("Postovi", new { temaId = post.TemaId });
            }
            return View(post);
        }

        public ActionResult UrediPost(int id)
        {
            var post = db.PopisPostova.Find(id);
            if(post == null || post.KorisnickoIme != User.Identity.Name)
            {
                return HttpNotFound();
            }
            return View(post);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UrediPost(Post post)
        {
            if (ModelState.IsValid)
            {
                var postojiPost = db.PopisPostova.Find(post.Id);
                if(postojiPost == null || postojiPost.KorisnickoIme != User.Identity.Name)
                {
                    return HttpNotFound();
                }

                postojiPost.Sadrzaj = post.Sadrzaj;
                db.SaveChanges();

                return RedirectToAction("Postovi", new { temaId = postojiPost.TemaId });
            }

            return View(post);
        }
    }
}