using BiciklistickiKlub.Servis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        private readonly EmailService _emailService;

        public MailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public ActionResult Index()
        {
            return View(new EmailViewModel());
        }

        [HttpPost]
        public ActionResult SendEmail(EmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                _emailService.SendEmail(model.ToEmail, model.Subject, model.Body);
                ViewBag.Message = "Email je uspešno poslat!";
                return RedirectToAction("Index");
            }
            return View("Index", model);
        }
    }
}