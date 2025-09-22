using WebProject.Models;
namespace WebProject.Services
{
        public interface IEmailService
        {
            Task<bool> SendEmailAsync(EmailRequest emailRequest);
        }
    
}
