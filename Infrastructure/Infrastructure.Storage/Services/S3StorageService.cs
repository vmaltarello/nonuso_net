using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Nonuso.Application.IServices;
using Nonuso.Infrastructure.Secret;

namespace Nonuso.Infrastructure.Storage.Services
{
    internal class S3StorageService(IAmazonS3 s3Client, ISecretManager secretManager) : IS3StorageService
    {
        readonly IAmazonS3 _s3Client = s3Client;
        readonly ISecretManager _secretManager = secretManager;

        public async Task<IEnumerable<string>> UploadProductImagesAsync(IEnumerable<IFormFile> images, Guid productId)
        {
            var uploadedUrls = new List<string>();

            foreach (var image in images)
            {
                var extension = Path.GetExtension(image.FileName);
                var uniqueSuffix = Guid.NewGuid().ToString()[..8];
                var fileName = $"{productId}_{uniqueSuffix}{extension}";

                using var stream = image.OpenReadStream();
                var putRequest = new PutObjectRequest
                {
                    BucketName = _secretManager.GetS3Settings().BucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = image.ContentType,
                    AutoCloseStream = true,
                    UseChunkEncoding = false
                };

                await _s3Client.PutObjectAsync(putRequest);

                var fileUrl = $"https://{_secretManager.GetS3Settings().BucketName}.nbg1.your-objectstorage.com/{fileName}";
                uploadedUrls.Add(fileUrl);
            }

            return uploadedUrls;
        }

        public async Task<IEnumerable<string>> RemoveProductImagesAsync(IEnumerable<string> images, Guid productId)
        {
            var existingImages = await GetProductImagesFromStorageAsync(productId);

            var imagesToDelete = existingImages.Where(existingUrl =>
                !images.Any(keepUrl => keepUrl.Equals(existingUrl, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            if (imagesToDelete.Count == 0) return existingImages;

            var deleteTaskList = imagesToDelete.Select(async imageUrl =>
            {
                var fileName = new Uri(imageUrl).Segments.Last();
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _secretManager.GetS3Settings().BucketName,
                    Key = fileName
                };

                try
                {
                    await _s3Client.DeleteObjectAsync(deleteRequest);
                }
                catch (Exception ex)
                {
                    // Log dell'errore, ma continua con le altre eliminazioni
                    // _logger.LogError(ex, "Errore durante l'eliminazione dell'immagine {ImageUrl}", imageUrl);
                }
            });

            await Task.WhenAll(deleteTaskList);

            return await GetProductImagesFromStorageAsync(productId);
        }

        #region PRIVATE

        private async Task<List<string>> GetProductImagesFromStorageAsync(Guid productId)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _secretManager.GetS3Settings().BucketName,
                Prefix = productId.ToString()
            };

            var existingImages = new List<string>();
            ListObjectsV2Response response;

            response = await _s3Client.ListObjectsV2Async(listRequest);

            if (response != null && response.S3Objects != null)
            {
                foreach (var obj in response.S3Objects)
                {
                    var fileUrl = $"https://{_secretManager.GetS3Settings().BucketName}.nbg1.your-objectstorage.com/{obj.Key}";
                    existingImages.Add(fileUrl);
                }
            }


            return existingImages;
        }

        #endregion
    }
}
