using System.Threading.Tasks;

namespace fa_api.Services.Mail
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}
