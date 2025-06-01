using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        await _service.RegisterAlertAsync(request);
        return Ok();
    }

    [HttpGet()]
    public async Task<IActionResult> GetAlerts()
    {
        var alerts = await _service.GetUserAlertsAsync();
        return Ok(alerts);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAlertAsync(id);
        return NoContent();
    }
}