using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IMapper _mapper;

    public AlertService(IAlertRepository alertRepository, IMapper mapper)
    {
        _alertRepository = alertRepository;
        _mapper = mapper;
    }

    public async Task RegisterAlertAsync(CreateAlertRequestDto request)
    {
        var alert = _mapper.Map<Alert>(request);
        alert.Id = Guid.NewGuid();
        //alert.CreatedAt = DateTime.UtcNow;
        alert.IsActive = true;

        await _alertRepository.AddAlertAsync(alert);
    }

    public async Task<IEnumerable<AlertDto>> GetUserAlertsAsync(Guid userId)
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync(userId);
        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }

    public async Task DeleteAlertAsync(Guid id)
    {
        await _alertRepository.DeleteAlertAsync(id);
    }
}