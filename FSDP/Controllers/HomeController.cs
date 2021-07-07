using FSDP.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel cvm)
        {
            string emailServer = WebConfigurationManager.AppSettings["EmailServer"];
            string emailPW = WebConfigurationManager.AppSettings["EmailPW"];
            string emailUser = WebConfigurationManager.AppSettings["EmailUser"];
            string emailToAddress = WebConfigurationManager.AppSettings["EmailToAddress"];

            if (!ModelState.IsValid)
            {
                return View(cvm);
            }

            string message = $"You have received a message from {cvm.Name} with a " +
                $"subject of {cvm.Subject}. Please respond to {cvm.EmailAddress} with " +
                $"your response to the following message:<br />{cvm.Message}";
            
            MailMessage mm = new MailMessage(emailUser, emailToAddress, cvm.Subject, cvm.Message);
            
            mm.IsBodyHtml = true; 
            mm.Priority = MailPriority.High;
            mm.ReplyToList.Add(cvm.EmailAddress);
            
            SmtpClient client = new SmtpClient(emailServer);
            client.Credentials = new NetworkCredential(emailUser, emailPW);
            
            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                ViewBag.CustomerMessage = $"Looks like something went wrong<br />Error message: {ex.Message}.";
                return View(cvm);
            }
            ViewBag.CustomerMessage = $"Thank you for your email. We typically resond within 7 days.";
            return View();
        }
    }
}
