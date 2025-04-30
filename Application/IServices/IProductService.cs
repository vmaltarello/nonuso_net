using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IProductService
    {
        Task CreateAsync(ProductParamModel model);
    }
}
