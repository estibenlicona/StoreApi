using AppStore.Core.DTOs;
using AppStore.Core.Entities;
using AutoMapper;

namespace AppStore.Infrastructure.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
