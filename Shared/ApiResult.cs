namespace Inventory.Data.Service.Shared
{
    public sealed class ApiResult<T>
    {
        public bool Success { get; init; }

        public T Data { get; init; } = default!;

        public string Message { get; init; } = string.Empty;

        public string TraceId { get; init; } = Guid.NewGuid().ToString("N");

        public IEnumerable<string> Errors { get; init; } = [];

        // Métodos de fábrica
        public static ApiResult<T> Ok(T data, string? message = null, object? meta = null) =>
            new()
            {
                Success = true,
                Data = data,
                Message = message ?? string.Empty           
            };

        public static ApiResult<T> Fail(IEnumerable<string> errors, string? message = null) =>
            new()
            {
                Success = false,
                Errors = errors ?? Array.Empty<string>(),
                Message = message ?? string.Empty,
                Data = default!
            };
    }
}
