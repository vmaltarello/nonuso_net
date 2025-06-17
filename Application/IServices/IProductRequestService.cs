using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IProductRequestService
    {
        Task CreateAsync(ProductRequestParamModel model);
    }
}
