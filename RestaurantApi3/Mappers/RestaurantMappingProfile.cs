using AutoMapper;
using RestaurantApi3.Dtos;
using RestaurantApi3.Entities;

namespace RestaurantApi3.Mappers;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(r => r.City, c => c.MapFrom(s => s.Address.City))
            .ForMember(r => r.PostalCode, c => c.MapFrom(s => s.Address.PostalCode))
            .ForMember(r => r.Street, c => c.MapFrom(s => s.Address.Street));

        CreateMap<Dish, DishDto>();

        CreateMap<CreateRestaurantDto, Restaurant>()
            .ForMember(r => r.Address,
                o => o.MapFrom(c => new Address() { Street = c.Street, City = c.City, PostalCode = c.PostalCode }));

        CreateMap<CreateDishDto, Dish>();

    }
    
}