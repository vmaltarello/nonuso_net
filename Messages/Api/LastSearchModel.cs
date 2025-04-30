using Nonuso.Messages.Api.Base;

namespace Nonuso.Messages.Api
{
    public class LastSearchModel : IModel
    {
        public Guid Id { get; set; }
        public string Search {  get; set; } = string.Empty;
    }

    public class LastSearchResultModel : LastSearchModel { }
}
