using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ServiceModel;



namespace Inventory.Data.Service.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Unhandled exception occurred.");
                var response = context.Response;
                response.ContentType = "application/json";

                var statusCode = error switch
                {
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    BadHttpRequestException or JsonException => (int)HttpStatusCode.BadRequest,
                    FaultException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var message = error switch
                {
                    ValidationException => "Validación fallida.",
                    BadHttpRequestException or JsonException => "El cuerpo de la solicitud no es un JSON válido.",
                    UnauthorizedAccessException => "No autorizado.",
                    FaultException fe => fe.Message,
                    KeyNotFoundException => "Recurso no encontrado.",
                    _ => "Error interno del servidor."
                };

                var errorDetails = new
                {
                    message,
                    error = error.Message,
                    stackTrace = error.StackTrace
                };

                response.StatusCode = statusCode;
                var result = JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                });

                await response.WriteAsync(result);
            }
        }
    }
}
