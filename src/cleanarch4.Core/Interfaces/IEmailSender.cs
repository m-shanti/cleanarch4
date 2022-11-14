using System.Threading.Tasks;

namespace cleanarch4.Core.Interfaces;

public interface IEmailSender
{
  Task SendEmailAsync(string to, string from, string subject, string body);
}
