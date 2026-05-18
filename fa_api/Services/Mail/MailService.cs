using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace fa_api.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly SmtpSettings _smtp;

        public MailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }

        public async Task SendAsync(MailRequest request)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
            message.To.Add(MailboxAddress.Parse(request.To));

            foreach (var cc in request.Cc)
                message.Cc.Add(MailboxAddress.Parse(cc));

            message.Subject = request.Subject;

            var bodyBuilder = new BodyBuilder();
            if (request.IsHtml)
                bodyBuilder.HtmlBody = request.Body;
            else
                bodyBuilder.TextBody = request.Body;

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            var secureOption = _smtp.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(_smtp.Host, _smtp.Port, secureOption);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
