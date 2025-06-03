using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPulse.API.Extensions;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;

namespace StockPulse.API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class StockPriceController : ControllerBase
    {
        private readonly IStockPricePublisherService _stockPricePublisherService;

        public StockPriceController(IStockPricePublisherService stockPricePublisherService)
        {
            _stockPricePublisherService = stockPricePublisherService;
        }

        [HttpPost]
        public async Task<IActionResult> AddStockPrice([FromBody] RecordPriceRequestDto request)
        {
            var response = await _stockPricePublisherService.RecordAndPublishAsync(request);
            return response.ToActionResult();
        }
    }
}
