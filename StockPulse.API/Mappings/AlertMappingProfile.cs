using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Domain.Entities;

namespace StockPulse.API.Mappings
{
    public class AlertMappingProfile : Profile
    {
        public AlertMappingProfile()
        {
            CreateMap<CreateAlertRequestDto, Alert>();
            CreateMap<Alert, AlertDto>();
            CreateMap<RecordPriceRequestDto, StockPrice>();
        }
    }
}
