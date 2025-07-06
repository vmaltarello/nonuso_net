namespace Nonuso.Application.IServices
{
    public interface IPresenceService
    {
        Task<(bool isOnline, string currentPage)?> GetUserPresenceAsync(Guid userId);
        Task SetUserOnlineAsync(Guid userId, string page);
        Task SetUserOfflineAsync(Guid userId);
        Task UpdatePageAsync(Guid userId, string page);
    }
}
