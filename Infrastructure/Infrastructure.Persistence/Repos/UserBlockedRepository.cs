using Nonuso.Domain.IRepos;

namespace Nonuso.Infrastructure.Persistence.Repos
{
    internal class UserBlockedRepository(NonusoDbContext context) : IUserBlockedRepository
    {
        private readonly NonusoDbContext _context = context;

    }
}
