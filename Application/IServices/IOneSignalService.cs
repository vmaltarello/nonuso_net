using Nonuso.Domain.Entities;

namespace Nonuso.Application.IServices
{
    public interface IOneSignalService
    {
        Task SendConfirmEmailAsync(User user, string tokenConfirmEmail);
    }
}
