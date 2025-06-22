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
            #region PRODUCT

            CreateMap<ProductDetailModel, ProductDetailResultModel>()
               .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl == null ? null : x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)))
               .ReverseMap();

            CreateMap<Product, EditProductParamModel>().ReverseMap();

            CreateMap<Product, ProductResultModel>()
                .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl == null ? null : x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)))
                .ReverseMap();

            CreateMap<Product, ProductModel>()
               .ForMember(dest => dest.ImagesURL, static opt => opt.MapFrom(x => x.ImagesUrl == null ? null : x.ImagesUrl.Split(ImageUrlSplitDelimiter, StringSplitOptions.RemoveEmptyEntries)))
               .ReverseMap();

            #endregion

            CreateMap<Domain.Models.ConversationModel, ConversationResultModel>();
            CreateMap<Conversation, ConversationResultModel>();
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<ProductRequest, Messages.Api.ProductRequestModel>();
            CreateMap<ChatModel, ChatResultModel>();
            CreateMap<MessageModel, ChatResultModel>();
            CreateMap<MessageModel, MessageResultModel>();
            CreateMap<Favorite, FavoriteResultModel>().ReverseMap();

            CreateMap<ReviewParamModel, Review>();
            CreateMap<ReviewResultModel, Review>().ReverseMap();
        }
    }
}
