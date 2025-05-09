using AutoMapper;
using PixelMart.API.Entities;
using PixelMart.API.Models.Stock;

namespace PixelMart.API.Profiles;

public class StocksProfile : Profile
{
    public StocksProfile()
    {
        CreateMap<StockManipulationDto, Stock>();

        CreateMap<Stock, StockItemDto>();
    }
}
