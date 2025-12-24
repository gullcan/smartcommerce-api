namespace SmartCommerce.Application.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public T? Data { get; init; }

    // Validation / field errors gibi durumlar için
    public Dictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "OK")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> OkEmpty(string message = "OK")
        => new() { Success = true, Message = message, Data = default };

    public static ApiResponse<T> Fail(string message)
        => new() { Success = false, Message = message, Data = default };

    public static ApiResponse<T> Fail(string message, Dictionary<string, string[]> errors)
        => new() { Success = false, Message = message, Data = default, Errors = errors };
}
