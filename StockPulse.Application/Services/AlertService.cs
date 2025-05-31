using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Domain.Entities;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IMapper _mapper;
    private readonly ISymbolValidator _symbolValidator;

    public AlertService(IAlertRepository alertRepository, IMapper mapper, ISymbolValidator symbolValidator)
    {
        _alertRepository = alertRepository;
        _mapper = mapper;
        _symbolValidator = symbolValidator;
    }

    public async Task RegisterAlertAsync(CreateAlertRequestDto request)
    {
        if (!_symbolValidator.IsValid(request.Symbol))
            throw new ArgumentException("Invalid stock symbol.");

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