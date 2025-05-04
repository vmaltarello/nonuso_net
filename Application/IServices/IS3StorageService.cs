using Microsoft.AspNetCore.Http;

namespace Nonuso.Application.IServices
{
    public interface IS3StorageService
    {
        Task<IEnumerable<string>> UploadProductImagesAsync(IEnumerable<IFormFile> images, Guid productId);
    }
}
