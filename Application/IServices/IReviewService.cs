using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IReviewService
    {
        Task CreateAsync(ReviewParamModel model);
    }
}
