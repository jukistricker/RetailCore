namespace RetailCore.Domain.Commons;

public class Result<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public string? Error { get; set; } = null;
    
    public static Result<T> Success(T value, int statusCode = 200) 
        => new() { IsSuccess = true, Value = value, StatusCode = statusCode };
    public static Result<T> Failure(string error, int statusCode = 400) 
        => new() { IsSuccess = false, Error = error, StatusCode = statusCode };
}