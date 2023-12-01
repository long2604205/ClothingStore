using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asm.Areas.Identity.Pages.Account.Manage;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using asm.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    public class ContactController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }
        // [HttpPost]
        // public IActionResult SendEmail(asm.Models.EmailModel model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             var message = new MimeMessage();
        //             message.From.Add(new MailboxAddress(model.SenderName, model.SenderEmail));
        //             message.To.Add(new MailboxAddress("Recipient Name", "recipient-email@example.com"));
        //             message.Subject = model.Subject;

        //             var builder = new BodyBuilder();
        //             builder.HtmlBody = model.Message;
        //             message.Body = builder.ToMessageBody();

        //             using (var client = new SmtpClient())
        //             {
        //                 client.Connect("smtp.gmail.com", 587, false);
        //                 client.Authenticate("duongchilong01887036681@gmail.com", "rocb puke douc pcbq");

        //                 client.Send(message);
        //                 client.Disconnect(true);
        //             }

        //             ViewBag.Message = "Email sent successfully!";
        //             return View();
        //         }
        //         catch (Exception ex)
        //         {
        //             ViewBag.Error = $"Error sending email: {ex.Message}";
        //             return View();
        //         }
        //     }

        //     return View();
        // }
        [HttpPost]
        public IActionResult SendEmail(asm.Models.EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(model.SenderName, model.SenderEmail));
                    message.To.Add(new MailboxAddress("Recipient Name", "recipient-email@example.com"));
                    message.Subject = model.Subject;

                    var builder = new BodyBuilder();

                    // Build a table with the form data
                    string tableHtml = $@"
                <table style='width:60%; border-collapse: collapse;'>
                    <tr>
                        <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Field</th>
                        <th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Value</th>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Name</td>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{model.SenderName}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Email</td>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{model.SenderEmail}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Subject</td>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{model.Subject}</td>
                    </tr>
                    <tr>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Message</td>
                        <td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{model.Message}</td>
                    </tr>
                </table>";

                    builder.HtmlBody = tableHtml;
                    message.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        client.Connect("smtp.gmail.com", 587, false);
                        client.Authenticate("duongchilong01887036681@gmail.com", "rocb puke douc pcbq");

                        client.Send(message);
                        client.Disconnect(true);
                    }

                    ViewBag.Type = "success";
                    ViewBag.Message = "Email sent successfully!";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Type = "danger";
                    ViewBag.Error = $"Error sending email: {ex.Message}";
                    return View();
                }
            }

            return View();
        }

    }
}

