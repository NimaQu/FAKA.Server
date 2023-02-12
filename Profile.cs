using AutoMapper;
using FAKA.Server.Models;
using FAKA.Server.Models.Dtos;

namespace FAKA.Server;

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
        CreateMap<GatewayInDto, Gateway>();

        CreateMap<Announcement, AnnouncementOutDto>();
        CreateMap<AnnouncementInDto, Announcement>();
        // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
    }
}