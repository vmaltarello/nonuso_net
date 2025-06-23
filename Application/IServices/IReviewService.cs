using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewResultModel>> GetAllAsync(Guid userId);
        Task CreateAsync(ReviewParamModel model);
    }
}
