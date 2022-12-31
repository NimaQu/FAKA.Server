using AutoMapper;
using faka.Models;
using faka.Models.DTO;

namespace faka;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Order, OrderDto>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)).ForAllMembers(opt => opt.Ignore());
        CreateMap<Product, ProductDto>().ForAllMembers(opt => opt.Ignore());
        CreateMap<Key, KeyDto>().ForAllMembers(opt => opt.Ignore());
        // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
    }
}