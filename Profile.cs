using AutoMapper;
using faka.Models;
using faka.Models.Dtos;

namespace faka;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Order, OrderOutDto>();
        CreateMap<OrderInDto, Order>();
        CreateMap<OrderSubmitDto, Order>();

        CreateMap<Product, ProductOutDto>();
        CreateMap<ProductInDto, Product>();

        CreateMap<Key, KeyOutDto>();
        CreateMap<KeyInDto, Key>();
        
        CreateMap<AssignedKey, AssignedKeyOutDto>();

        CreateMap<ProductGroupInDto, ProductGroup>();
        CreateMap<ProductGroup, ProductGroupOutDto>();

        CreateMap<TransactionInDto, Transaction>();

        CreateMap<Gateway, GatewayOutDto>();
        // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
    }
}