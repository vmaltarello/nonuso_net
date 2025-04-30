using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class CategoryModel : IModel
    {
        public Guid Id {  get; set; }
        public required string Description  { get; set; }
        public required string DescriptionEn {  get; set; }
    }

    public class CategoryResultModel : CategoryModel { }
}
