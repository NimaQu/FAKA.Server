using AutoMapper;
using faka.Models;
using faka.Models.Dtos;

namespace faka;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Order, OrderOutDto>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)).ForAllMembers(opt => opt.UseDestinationValue());
        CreateMap<OrderInDto, Order>();
        CreateMap<OrderSubmitDto, Order>();
        
        CreateMap<Product, ProductOutDto>().ForAllMembers(opt => opt.UseDestinationValue());
        CreateMap<ProductInDto, Product>();
        
        CreateMap<Key, KeyOutDto>().ForAllMembers(opt => opt.UseDestinationValue());
        CreateMap<KeyInDto, Key>();

        CreateMap<ProductGroupInDto, ProductGroup>();
        CreateMap<ProductGroup, ProductGroupOutDto>()
            .ForMember(dto => dto.Products, opt => opt.MapFrom(src => src.Products));
        
        CreateMap<TransactionInDto, Transaction>();
        
        CreateMap<Gateway, GatewayOutDto>().ForAllMembers(opt => opt.UseDestinationValue());
        // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
    }
}