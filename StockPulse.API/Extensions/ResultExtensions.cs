using Microsoft.AspNetCore.Mvc;
using StockPulse.Application.Enums;
using StockPulse.Application.Models;

namespace StockPulse.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            return result.StatusCode switch
            {
                StatusCode.Success => new OkObjectResult(result.Message),
                StatusCode.BadRequest => new BadRequestObjectResult(result.Message),
                StatusCode.NotFound => new NotFoundObjectResult(result.Message),
                StatusCode.Conflict => new ConflictObjectResult(result.Message),
                StatusCode.InternalServerError => new ObjectResult(result.Message) { StatusCode = 500 },
                _ => new ObjectResult(result.Message) { StatusCode = (int)result.StatusCode }
            };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result.StatusCode switch
            {
                StatusCode.Success => new OkObjectResult(result.Data),
                StatusCode.BadRequest => new BadRequestObjectResult(result.Message),
                StatusCode.NotFound => new NotFoundObjectResult(result.Message),
                StatusCode.Conflict => new ConflictObjectResult(result.Message),
                StatusCode.InternalServerError => new ObjectResult(result.Message) { StatusCode = 500 },
                _ => new ObjectResult(result.Message) { StatusCode = (int)result.StatusCode }
            };
        }
    }
}
