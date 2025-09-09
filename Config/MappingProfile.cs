using Inventory.Data.Service.DTOs;
using AutoMapper;
using Inventory.Data.Service.Models;
using System.Text.Json;

namespace Inventory.Data.Service.Mappings
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductModel>();
            CreateMap<Store, StoreModel>();
            CreateMap<DTOs.Inventory, InventoryModel>();
            CreateMap<Sale, SaleOccurredEvent>();
            //CreateMap<SaleOccurredEvent, QueuedMessageModel>();

            CreateMap<SaleOccurredEvent, QueuedMessageModel>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.Payload, opt => opt.MapFrom(src =>
                 JsonSerializer.Serialize(src, (JsonSerializerOptions)null) ?? string.Empty))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pendiente"))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
