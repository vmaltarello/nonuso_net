using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Nonuso.Application.IServices;

namespace Nonuso.Infrastructure.Storage.Services
{
    internal class S3StorageService(IAmazonS3 s3Client, IConfiguration configuration) : IS3StorageService
    {
        readonly IAmazonS3 _s3Client = s3Client;
        readonly IConfiguration _configuration = configuration;

        public async Task<IEnumerable<string>> UploadProductImagesAsync(IEnumerable<IFormFile> images, Guid productId)
        {
            var uploadedUrls = new List<string>();
            var bucketName = _configuration["S3HetznerStorage:BucketName"];

            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName);
                var uniqueSuffix = Guid.NewGuid().ToString()[..8];
                var fileName = $"{productId}_{uniqueSuffix}{extension}";

                using var stream = image.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = image.ContentType,
                    AutoCloseStream = true,
                    UseChunkEncoding = false
                };

                await _s3Client.PutObjectAsync(putRequest);

                var fileUrl = $"https://{bucketName}.nbg1.your-objectstorage.com/{fileName}";
                uploadedUrls.Add(fileUrl);
            }

            return uploadedUrls;
        }
    }
}
