using Nonuso.Application.IServices;
using Nonuso.Domain.IRepos;
using System.Text.RegularExpressions;

namespace Nonuso.Infrastructure.Redis.Services
{
    internal class PresenceService(IPresenceRepository presenceRepository,
        IProductRepository productRepository) : IPresenceService
    {
        readonly IPresenceRepository _presenceRepository = presenceRepository;
        readonly IProductRepository _productRepository = productRepository;

        private static readonly Regex ProductDetailPattern =
        new Regex(@"^/product-list/product-detail/([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})$",
                  RegexOptions.Compiled);

        public async Task<(bool isOnline, string currentPage)?> GetUserPresenceAsync(Guid userId)
        {
            return await _presenceRepository.GetUserPresenceAsync(userId);
        }

        public async Task SetUserOfflineAsync(Guid userId)
        {
            await _presenceRepository.SetUserOfflineAsync(userId);
        }

        public async Task SetUserOnlineAsync(Guid userId, string page)
        {
            await _presenceRepository.SetUserOnlineAsync(userId, page);

            if (TryExtractGuid(page, out Guid productId))
            {
                await UpdateProductViewCount(userId, productId);
            }
        }

        public async Task UpdatePageAsync(Guid userId, string page)
        {
            await _presenceRepository.UpdatePageAsync(userId, page);

            if (TryExtractGuid(page, out Guid productId))
            {
                await UpdateProductViewCount(userId, productId);
            }
        }

        #region PRIVATE

        private async Task UpdateProductViewCount(Guid userId, Guid productId)
        {
            var entity = await _productRepository.GetByIdAsync(productId);

            if (entity == null || entity.UserId == userId) return;

            entity.ViewCount += 1;

            await _productRepository.UpdateAsync(entity);
        }

        private static bool TryExtractGuid(string input, out Guid guid)
        {
            guid = Guid.Empty;

            if (string.IsNullOrEmpty(input))
                return false;

            var match = ProductDetailPattern.Match(input);
            if (match.Success)
            {
                return Guid.TryParse(match.Groups[1].Value, out guid);
            }

            return false;
        }

        #endregion
    }
}
