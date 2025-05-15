namespace Nonuso.Common.Filters
{
    public class ProductFilter
    {
        public int Page { get; set; } = 0;
        public Guid? CategoryId { get; set; }
        public Guid? UserId { get; set; }
        public string? Search { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public double? Distance { get; set; }
    }
}
