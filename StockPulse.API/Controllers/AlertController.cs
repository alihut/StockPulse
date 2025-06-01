using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPulse.API.Extensions;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly IAlertService _service;

    public AlertController(IAlertService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAlert([FromBody] CreateAlertRequestDto request)
    {
        var response = await _service.RegisterAlertAsync(request);
        return response.ToActionResult();
    }

    [HttpGet()]
    public async Task<IActionResult> GetAlerts()
    {
        var response = await _service.GetUserAlertsAsync();
        return response.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _service.DeleteAlertAsync(id);
        return response.ToActionResult();
    }
}