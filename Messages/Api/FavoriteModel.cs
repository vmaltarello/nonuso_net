using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class FavoriteModel : IModel
    {
        public Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid ProductId { get; set; }

    }

    public class FavoriteResultModel : FavoriteModel
    {
        public required ProductResultModel Product { get; set; }
    }
}