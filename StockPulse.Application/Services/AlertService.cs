using AutoMapper;
using StockPulse.Application.DTOs;
using StockPulse.Application.Enums;
using StockPulse.Application.Interfaces;
using StockPulse.Application.Models;
using StockPulse.Domain.Entities;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IMapper _mapper;
    private readonly ISymbolValidator _symbolValidator;
    private readonly IUserContext _userContext;

    public AlertService(
        IAlertRepository alertRepository, 
        IMapper mapper, 
        ISymbolValidator symbolValidator,
        IUserContext userContext)
    {
        _alertRepository = alertRepository;
        _mapper = mapper;
        _symbolValidator = symbolValidator;
        _userContext = userContext;
    }

    public async Task<Result<Guid>> RegisterAlertAsync(CreateAlertRequestDto request)
    {
        if (!_symbolValidator.IsValid(request.Symbol))
            return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid stock symbol.");

        if(request.PriceThreshold < 0)
            return Result.Failure<Guid>(StatusCode.BadRequest, "Invalid PriceThreshold");

        var userId = _userContext.GetUserId();

        var alreadyExists = await _alertRepository.ExistsAsync(userId, request.Symbol, request.PriceThreshold, request.Type);
        if (alreadyExists)
            return Result.Failure<Guid>(StatusCode.Conflict, "An identical alert already exists.");

        var alert = _mapper.Map<Alert>(request);
        alert.IsActive = true;
        alert.UserId = _userContext.GetUserId();

        await _alertRepository.AddAlertAsync(alert);

        return Result.Success<Guid>(alert.Id);
    }

    public async Task<Result<IEnumerable<AlertDto>>> GetUserAlertsAsync()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync(_userContext.GetUserId());
        return Result.Success(_mapper.Map<IEnumerable<AlertDto>>(alerts));
    }

    public async Task<Result> DeleteAlertAsync(Guid id)
    {
        await _alertRepository.DeleteAlertAsync(id);

        return Result.Success();
    }
}