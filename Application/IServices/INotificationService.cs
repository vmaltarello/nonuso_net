using Nonuso.Domain.Entities;
using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface INotificationService
    {
        Task SendConfirmEmailAsync(User user, string link);
        Task SendRequestResetPasswordEmailAsync(User user, string link);
        Task SendPushNotificationAsync(PusNotificationParamModel model);
    }
}
