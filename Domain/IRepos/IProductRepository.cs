using Nonuso.Domain.Entities;

namespace Nonuso.Domain.IRepos
{
    public interface IProductRepository
    {
        Task CreateAsync(Product entity);
    }
}
