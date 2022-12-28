using AutoMapper;
using faka.Models;
using faka.Models.DTO;

namespace faka;

public class OrganizationProfile : Profile
{
    public OrganizationProfile()
    {
        CreateMap<Order, OrderDto>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        // Use CreateMap... Etc.. here (Profile methods are the same as configuration methods)
    }
}