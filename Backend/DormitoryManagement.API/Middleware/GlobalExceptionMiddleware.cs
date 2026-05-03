using DormitoryManagement.API.Models;

namespace DormitoryManagement.API.Middleware;

/// <summary>
/// Global exception-handling middleware.
/// Sits at the top of the pipeline and converts every unhandled exception
/// into a structured JSON error response with an appropriate HTTP status code.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate                       _next;
    private readonly ILogger<GlobalExceptionMiddleware>    _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate                     next,
        ILogger<GlobalExceptionMiddleware>  logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. RequestId={RequestId} Method={Method} Path={Path}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            Exceptions.NotFoundException    nfe => (StatusCodes.Status404NotFound,            nfe.Message),
            Exceptions.ValidationException  ve  => (StatusCodes.Status400BadRequest,          ve.Message),
            _                                   => (StatusCodes.Status500InternalServerError,
                                                   "An unexpected error occurred. Please try again later.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            await TriggerCriticalAlertAsync(exception, context);

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            StatusCode = statusCode,
            Message    = message,
            TraceId    = context.TraceIdentifier
        });
    }

    /// <summary>
    /// Hook for critical system alerts after an unhandled 500 error.
    ///
    /// Production integrations to consider:
    ///   • PagerDuty      — POST to Events API v2 (severity: critical)
    ///   • Slack          — POST to an incoming webhook with error summary
    ///   • Email (SMTP)   — via IEmailSender / MailKit
    ///   • GCP Error Reporting / Azure Application Insights — trackException()
    ///
    /// All of these would be injected via IOptions or dedicated service adapters.
    /// </summary>
    private Task TriggerCriticalAlertAsync(Exception exception, HttpContext context)
    {
        _logger.LogCritical(exception,
            "CRITICAL SYSTEM ERROR — Immediate attention required. " +
            "RequestId={RequestId} Path={Path}",
            context.TraceIdentifier,
            context.Request.Path);

        // TODO: inject and call IAlertService here
        return Task.CompletedTask;
    }
}
