using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers;
    public class LostEmailController : Controller
    {
        private readonly MailSettings _mailSettings;
        public LostEmailController(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string recipient, string subject, string body, IFormFile attachment)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.FromName, _mailSettings.FromAddress));
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;

            if (attachment != null && attachment.Length > 0)
            {
                var attachmentName = attachment.FileName;
                var attachmentContent = new MimeContent(attachment.OpenReadStream(), ContentEncoding.Default);
                var attachmentEntity =  new MimePart("mixed", "attachment")
                {
                    Content = new MimeContent(attachmentContent.Stream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachmentName
                };
                message.Body = new Multipart("mixed")
                {
                    new TextPart("plain")
                    {
                        Text = body
                    },
                    attachmentEntity
                };
            }
            else
            {
                message.Body = new TextPart("plain")
                {
                    Text = body
                };
            }
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.SmtpPort, false);
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return RedirectToAction("Index", "LostItems");
        }
    }
