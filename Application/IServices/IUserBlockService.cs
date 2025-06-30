using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IUserBlockService
    {
        Task BlockAsync(UserBlockParamModel model);
        Task UnBlockAsync(Guid id);
        Task<CheckUserBlockResultModel> CheckBlockAsync(CheckUserBlockParamModel model);
    }
}
