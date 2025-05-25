namespace Nonuso.Application.IServices
{
    public interface IPresenceService
    {
        Task SetUserOnlineAsync(Guid userId, string page);
        Task SetUserOfflineAsync(Guid userId);
        Task UpdatePageAsync(Guid userId, string page);
    }
}
