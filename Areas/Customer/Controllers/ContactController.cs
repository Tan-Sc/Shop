using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using System.Net.Mail;
using System.Net;

//using MimeKit;

namespace doan_webfix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {

        //[BindProperty]
        //public SendEmail sendemail { get; set; }

        public ContactController()
        {
           
        }

        [HttpGet]
        public IActionResult Index()
        {

            return View();
        }





        //[HttpPost, ActionName("Index")]
        //public IActionResult IndexPOST()
        //{
        //    SendMail();
        //    return RedirectToAction(nameof(Index));
        //}
        //private void SendMail()
        //{
        //    var credentials = new NetworkCredential("conchodien1207@gmail.com", "0933865847hai");

        //    var mail = new MailMessage()
        //    {
        //        From = new MailAddress("conchodien1207@gmail.com"),
        //        Subject = "Phana hoi cua khach hang",
        //        Body = sendemail.Message,
        //    };

        //    mail.IsBodyHtml = true;
        //    mail.To.Add(new MailAddress("conchodien1207@gmail.com"));

        //    var client = new SmtpClient()
        //    {
        //        Port = 587,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Host = "smtp.gmail.com",
        //        EnableSsl = true,
        //        Credentials = credentials

        //    };
        //    client.Send(mail);



            //var mail = new MimeMessage();
            //mail.From.Add(new MailboxAddress("Rio Coffee & Food", "conchodien1207@gmail.com"));
            //mail.To.Add(new MailboxAddress("Rio", sendemail.Email));
            //mail.Subject = "Rio Coffee & Food - Reset Your Password";

            //mail.Body = new TextPart("plain")
            //{
            //    Text = "Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
            //};

            //using (var client = new MailKit.Net.Smtp.SmtpClient())
            //{
            //    client.Connect("smtp.gmail.com", 587, false);
            //    client.Authenticate("conchodien1207@gmail.com", "0933865847hai");
            //    client.Send(mail);
            //    client.Disconnect(true);
            //}


      //  }
    }
}
