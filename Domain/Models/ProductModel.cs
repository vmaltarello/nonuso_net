using Nonuso.Domain.Entities;

namespace Nonuso.Domain.Models
{
    public class ProductDetailModel : Product
    {
        public bool IsMyFavorite { get; set; }
        public int FavoriteCount { get; set; }
    }
}
