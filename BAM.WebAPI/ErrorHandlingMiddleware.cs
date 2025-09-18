using System.Text.Json;

namespace BAM.WebAPI
{
    internal class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            try
            {
                _logger.LogError(exception, "An unhandled exception occurred while processing the request.");

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handling middleware will not modify the response.");
                    return;
                }

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json; charset=utf-8";

                var payload = new
                {
                    error = "An unexpected error occurred.",
                    // include a short message for diagnostics; avoid exposing stack traces or sensitive info
                    details = exception.Message
                };

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(payload, options);
                await context.Response.WriteAsync(json).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // If an exception occurs while handling the original exception, log it.
                _logger.LogError(ex, "An exception occurred while writing the error response.");
                // We cannot do much more here; the request pipeline will terminate.
            }
        }
    }

    internal static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
