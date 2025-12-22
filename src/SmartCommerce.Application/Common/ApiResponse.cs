namespace SmartCommerce.Application.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "OK")
        => new ApiResponse<T> { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message)
        => new ApiResponse<T> { Success = false, Message = message, Data = default };
}
