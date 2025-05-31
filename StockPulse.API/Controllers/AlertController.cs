using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPulse.Application.DTOs;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AlertController : ControllerBase
{
    private readonly AlertService _service;

    public AlertController(AlertService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterAlert([FromBody] CreateAlertRequestDto request)
    {
        await _service.RegisterAlertAsync(request);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAlerts(Guid userId)
    {
        var alerts = await _service.GetUserAlertsAsync(userId);
        return Ok(alerts);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAlertAsync(id);
        return NoContent();
    }
}