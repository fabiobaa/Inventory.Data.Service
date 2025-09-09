using System.Diagnostics;

namespace Inventory.Data.Service.Middleware
{
    public class PerformanceMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMetricsMiddleware> _logger;

        public PerformanceMetricsMiddleware(RequestDelegate next, ILogger<PerformanceMetricsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Inicia el cronómetro
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Pasa la petición al siguiente componente en el pipeline (ej. el controlador)
                await _next(context);
            }
            finally
            {
                // Detiene el cronómetro una vez que la respuesta está lista
                stopwatch.Stop();

                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                var requestPath = $"{context.Request.Method} {context.Request.Path}";

                // Registra el resultado en los logs
                _logger.LogInformation(
                    "Request {Path} finished in {ElapsedMilliseconds}ms with status {StatusCode}",
                    requestPath,
                    elapsedMilliseconds,
                    context.Response.StatusCode);
            }
        }
    }
}
