using Quizzz.Models;

namespace Quizzz.UtilityService
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
