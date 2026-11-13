using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services.Exceptions;

namespace Varjupaik.Api.ErrorHandling;

// Üks koht, kus kõik erandid muudetakse ühtseks ProblemDetails vastuseks
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetails;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetails, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetails = problemDetails;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        string title;
        string detail;

        switch (exception)
        {
            case NotFoundException:
                status = StatusCodes.Status404NotFound;
                title = "Not Found";
                detail = exception.Message;
                _logger.LogWarning("404 {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, exception.Message);
                break;

            case BadRequestException:
                status = StatusCodes.Status400BadRequest;
                title = "Bad Request";
                detail = exception.Message;
                _logger.LogWarning("400 {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, exception.Message);
                break;

            case UnauthorizedException:
                status = StatusCodes.Status401Unauthorized;
                title = "Unauthorized";
                detail = exception.Message;
                _logger.LogWarning("401 {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, exception.Message);
                break;

            default:
                // Ootamatu viga: logime kogu info, kliendile ei näita sisemisi detaile
                status = StatusCodes.Status500InternalServerError;
                title = "Internal Server Error";
                detail = "Serveris tekkis ootamatu viga. Proovi hiljem uuesti.";
                _logger.LogError(exception, "500 {Method} {Path}: käsitlemata viga", context.Request.Method, context.Request.Path);
                break;
        }

        context.Response.StatusCode = status;

        return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            }
        });
    }
}
