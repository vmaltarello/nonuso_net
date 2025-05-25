using Nonuso.Domain.Entities;
using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface INotificationService
    {
        Task SendConfirmEmailAsync(User user, string tokenConfirmEmail);
        Task SendPushNotificationAsync(PusNotificationParamModel model);
    }
}
