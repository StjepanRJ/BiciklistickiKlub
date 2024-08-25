using BiciklistickiKlub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BiciklistickiKlub.Controllers
{
    public class GmailController : Controller
    {
        private readonly EmailService _emailService;

        public GmailController()
        {
            _emailService = new EmailService();
        }

        [HttpPost]
        public async Task<ActionResult> SendEmail(string toEmail, string subject, string message)
        {
            if (ModelState.IsValid)
            {
                await _emailService.SendEmailAsync(toEmail, subject, message);
                ViewBag.Message = "Email je uspješno poslan!";
                return View("Index");
            }
            return View("Index");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}