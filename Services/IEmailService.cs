using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Services
{
    public interface IEmailService
    {
        Task SendContactFormEmailAsync(string recipient, string subject, string name, string email, string message);
    }
} 