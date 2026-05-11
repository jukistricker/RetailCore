namespace RetailCore.Domain.Commons;

public class Result<T>
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public List<string>? Errors { get; set; } // Đổi tên thành số nhiều cho chuẩn

    public static Result<T> Success(T value, int statusCode = 200)
    {
        return new Result<T>
        {
            IsSuccess = true, 
            Value = value, 
            StatusCode = statusCode 
        };
    }

    public static Result<T> Failure(List<string> errors, int statusCode = 400)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors,
            StatusCode = statusCode
        };
    }
    
    // Overloading nhận vào 1 chuỗi lỗi duy nhất (Dùng cho lỗi logic đơn lẻ)
    public static Result<T> Failure(string error, int statusCode = 400)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = new List<string>{error},
            StatusCode = statusCode
        };
    } 
}