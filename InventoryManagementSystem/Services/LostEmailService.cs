using System.Net;
using System.Net.Mail;

public class LostEmailService
{
    private readonly string _fromAddress;
    private readonly string _fromPassword;
    private readonly string _smtpServer;
    private readonly int _smtpPort;

    public LostEmailService(string fromAddress, string fromPassword, string smtpServer, int smtpPort)
    {
        _fromAddress = fromAddress;
        _fromPassword = fromPassword;
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
    }

    public void SendEmail(string recipient, string subject, string message)
    {
        var fromAddress = new MailAddress(_fromAddress);
        var toAddress = new MailAddress(recipient);

        var smtp = new SmtpClient
        {
            Host = _smtpServer,
            Port = _smtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("5491a7aee6a18f", "fb439a85ffbe28")
        };

        using (var mailMessage = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        })
        {
            smtp.Send(mailMessage);
        }
    }
}
