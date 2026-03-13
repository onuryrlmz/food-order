using AutoMapper;
using Domain.Dto.Buyer;
using Domain.Dto.Common;
using Domain.Dto.Seller.Category;
using Domain.Dto.Seller.CategoryDetail;
using Domain.Dto.Seller.Menu;
using Domain.Dto.Seller.MenuOption;
using Domain.Dto.Seller.MenuOptionValue;
using Domain.Dto.Seller.MenuOptionValueOption;
using Domain.Dto.Seller.MenuOptionValueOptionValue;
using Domain.Dto.Seller.OptionTemplate;
using Domain.Dto.Seller.Product;
using Domain.Dto.Seller.Restaurant;
using Domain.Entities.Buyer;
using Domain.Entities.Common;
using Domain.Entities.Seller;

namespace Application.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<Address, AddAddressDto>().ReverseMap();
        CreateMap<Address, GetAddressDto>().ReverseMap();
        CreateMap<Address, UpdateAddressDto>().ReverseMap();
        CreateMap<Restaurant, AddRestaurantDto>().ReverseMap();
        CreateMap<Product, AddProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
        CreateMap<Product, ProductResponseDto>().ReverseMap();
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

        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<Category, CreateCategoryRequestDto>().ReverseMap();
        CreateMap<Category, UpdateCategoryRequestDto>().ReverseMap();
        CreateMap<CategoryDetail, CategoryDetailResponse>().ReverseMap();
        CreateMap<CategoryDetail, CreateCategoryDetailRequestDto>().ReverseMap();

        CreateMap<OptionTemplate, OptionTemplateResponseDto>().ReverseMap();
        CreateMap<OptionTemplateValue, OptionTemplateValueResponseDto>().ReverseMap();
        CreateMap<OptionTemplateValueOption, OptionTemplateValueOptionResponseDto>().ReverseMap();
        CreateMap<OptionTemplateValueOptionValue, OptionTemplateValueOptionValueResponseDto>().ReverseMap();

        CreateMap<BasketItemValueItemValue, GetBasketDto.GetBasketItemDto.GetBasketItemValueDto.GetBasketItemValueItemValueDto>().ReverseMap();
        CreateMap<BasketItemValue, GetBasketDto.GetBasketItemDto.GetBasketItemValueDto>().ReverseMap();
        CreateMap<BasketItem, GetBasketDto.GetBasketItemDto>().ReverseMap();
        CreateMap<Basket, GetBasketDto>().ReverseMap();
    }
}