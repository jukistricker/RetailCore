namespace RetailCore.Domain.Commons;

public class Result<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public List<ErrorDetail>? Errors { get; set; }

    public static Result<T> Success(T value, int statusCode = 200)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = value,
            StatusCode = statusCode
        };
    }

    public static Result<T> Failure(List<ErrorDetail> errors, int statusCode = 400)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    // Overloading nhận vào 1 chuỗi lỗi duy nhất (Dùng cho lỗi logic đơn lẻ)
    public static Result<T> Failure(string? key, string message, int statusCode = 400)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = new List<ErrorDetail> { new() { Key = key, Message = message } },
            StatusCode = statusCode
        };
    }
}

public class ErrorDetail
{
    public string Key { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}