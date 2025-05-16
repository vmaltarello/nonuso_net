using AutoMapper;
using Nonuso.Domain.Entities;
using Nonuso.Domain.Models;
using Nonuso.Messages.Api;

namespace Nonuso.Application
{
    internal class Mapper : Profile
    {
        // Define a static readonly field for the array to avoid CA1861  
        private static readonly string[] ImageUrlSplitDelimiter = [","];

        internal Mapper()
        {
            CreateMap<Domain.Models.ConversationModel, ConversationResultModel>();

            CreateMap<ProductDetailModel, ProductDetailResultModel>()
                .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)));

            CreateMap<Product, ProductResultModel>()
                .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)));

            CreateMap<Product, ProductModel>()
               .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
