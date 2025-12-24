namespace SmartCommerce.Application.Common.Validation;

public static class ValidationErrors
{
    public static Dictionary<string, string[]> Single(string field, string message)
        => new() { [field] = new[] { message } };
}
