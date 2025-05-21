using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRequestRepository
    {
        Task CreateAsync(ProductRequest entity);
    }
}
