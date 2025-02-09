using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.APIs.ImageResolvers;
using Talabat.Core.Entities;

namespace Talabat.APIs.Profiles
{
    public class ProductProfile : BaseProfile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(PD => PD.ProductBrand, MemberConf => MemberConf.MapFrom(P => P.ProductBrand.Name))
                .ForMember(PD => PD.ProductType, MemberConf => MemberConf.MapFrom(P => P.ProductType.Name))
                .ForMember(PD => PD.PictureUrl, MemberConf => MemberConf.MapFrom<ProductPictureUrlResolver>());
        }
    }
}
