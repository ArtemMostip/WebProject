using WebProject.Models;
namespace WebProject.Repositories

{
        public interface IEmailRepository
        {
            Task<bool> SendEmailAsync(EmailRequest emailRequest);
        }

}
