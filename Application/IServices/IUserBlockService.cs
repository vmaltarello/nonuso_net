using Nonuso.Messages.Api;

namespace Nonuso.Application.IServices
{
    public interface IUserBlockService
    {
        Task Block(UserBlockParamModel model);
        Task UnBlock(Guid id);
    }
}
