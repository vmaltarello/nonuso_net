using Microsoft.AspNetCore.Http;
using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class ProductBaseModel : IModel
	{
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required Guid CategoryId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public required string LocationName { get; set; }
      
    }

    public class ProductModel : ProductBaseModel
    {
		// Place
    }

	public class ProductParamModel : ProductModel 
    {
        public IEnumerable<IFormFile> Images { get; set; } = [];
    }

    public class ProductResultModel : ProductModel 
    {
        public string[] ImagesURL { get; set; } = [];
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}