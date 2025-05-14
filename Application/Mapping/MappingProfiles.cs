using AutoMapper;
using Domain.Dto.Common;
using Domain.Dto.Seller;
using Domain.Dto.Seller.Menu;
using Domain.Dto.Seller.MenuOption;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Entities.Common;
using Domain.Entities.Seller;

namespace Application.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<Address, AddAddressDto>().ReverseMap();
        CreateMap<Restaurant, AddRestaurantDto>().ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
        CreateMap<Menu, MenuResponseDto>().ReverseMap();
        CreateMap<Menu, CreateMenuRequestDto>().ReverseMap();
        CreateMap<MenuOption, MenuOptionResponseDto>().ReverseMap();
        CreateMap<MenuOption, CreateMenuOptionRequestDto>().ReverseMap();
        CreateMap<MenuOptionValue, MenuOptionValueResponseDto>().ReverseMap();
        CreateMap<MenuOptionValue, CreateMenuOptionValueRequestDto>().ReverseMap();
        CreateMap<MenuOptionValueOption, MenuOptionValueOptionResponseDto>().ReverseMap();
        CreateMap<MenuOptionValueOption, CreateMenuOptionValueOptionRequestDto>().ReverseMap();
        CreateMap<MenuOptionValueOptionValue, MenuOptionValueOptionValueResponseDto>().ReverseMap();
        CreateMap<MenuOptionValueOptionValue, CreateMenuOptionValueOptionValueRequestDto>().ReverseMap();
    }
}