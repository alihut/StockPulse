using StockPulse.Application.Enums;

namespace StockPulse.Application.Models
{
    public class Result
    {
        public StatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public bool IsSuccess => (int)StatusCode >= 200 && (int)StatusCode < 300;

        public static Result Success(string message = "Success")
        {
            return new Result
            {
                StatusCode = StatusCode.Success,
                Message = message
            };
        }

        public static Result Failure(StatusCode statusCode, string message)
        {
            return new Result
            {
                StatusCode = statusCode,
                Message = message
            };
        }

        public static Result<T> Success<T>(T data, string message = "Success")
        {
            return new Result<T>
            {
                StatusCode = StatusCode.Success,
                Message = message,
                Data = data
            };
        }

        public static Result<T> Failure<T>(StatusCode statusCode, string message)
        {
            return new Result<T>
            {
                StatusCode = statusCode,
                Message = message,
                Data = default
            };
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }
    }
}
