namespace Nonuso.Domain.IRepos
{
    public interface IPresenceRepository
    {
        Task GetUserPresenceAsync(Guid userId);
        Task SetUserOnlineAsync(Guid userId, string page);
        Task SetUserOfflineAsync(Guid userId);
        Task UpdatePageAsync(Guid userId, string page);
    }
}
