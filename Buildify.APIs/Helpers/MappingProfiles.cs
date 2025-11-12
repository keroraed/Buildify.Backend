using AutoMapper;
using Buildify.Core.DTOs;
using Buildify.Core.Entities;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // User and Address mappings
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<CreateAddressDto, Address>();
        CreateMap<UpdateAddressDto, Address>();
        CreateMap<AppUser, AccountProfileDto>();

        // Category mappings
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>();

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Cart mappings
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => new ShippingAddressDto
            {
                FirstName = src.ShippingFirstName,
                LastName = src.ShippingLastName,
                Street = src.ShippingStreet,
                City = src.ShippingCity,
                State = src.ShippingState,
                ZipCode = src.ShippingZipCode,
                Country = src.ShippingCountry
            }));
        CreateMap<OrderItem, OrderItemDto>();

        // Dashboard mappings
        CreateMap<Order, RecentOrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<Product, LowStockProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
}
