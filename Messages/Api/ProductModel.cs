using Microsoft.AspNetCore.Http;
using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class ProductLocationModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public required string LocationName { get; set; }
    }

    public class ProductModel : IModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Guid CategoryId { get; set; }
        public string[] ImagesURL { get; set; } = [];
    }

    public class ProductParamModel : ProductLocationModel
    {
        public Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Guid CategoryId { get; set; }
        public IEnumerable<IFormFile> Images { get; set; } = [];
    }

    public class EditProductParamModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Guid CategoryId { get; set; }
        public IEnumerable<IFormFile> Images { get; set; } = [];
        public List<string> ExistingImages { get; set; } = [];
    }

    public class ProductResultModel : ProductModel
    {
        public required string LocationName { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProductDetailResultModel : ProductResultModel
    {
        public string UserName { get; set; } = null!;
        public bool IsMyFavorite { get; set; } = false;
        public bool IsMyProduct { get; set; } = false;
        public int FavoriteCount { get; set; } = 0;

    }
}