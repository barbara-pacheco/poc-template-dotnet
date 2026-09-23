using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sample.Domain._Shared;

namespace Sample.WebApi._Shared;

/// <summary>
/// Rede de segurança: captura qualquer exceção que escape do pipeline
/// (bug, validação de entrada, invariante de domínio violada) e traduz pra
/// Problem Details, sempre. Mecanismo nativo do ASP.NET Core (IExceptionHandler,
/// desde a versão 8) — nunca roda pra falha esperada devolvida como ErrorOr,
/// só pra exceção de verdade (throw).
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request cancelled by the client: {TraceId}", httpContext.TraceIdentifier);
            return true;
        }

        var (statusCode, title, extensions) = Describe(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        extensions["traceId"] = httpContext.TraceIdentifier;
        httpContext.Response.StatusCode = statusCode;

        var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Extensions = extensions!
            }
        });
    }

    private static (int StatusCode, string Title, Dictionary<string, object?> Extensions) Describe(Exception exception) =>
        exception switch
        {
            BadHttpRequestException badHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Failed to read the request body.",
                new Dictionary<string, object?> { ["detail"] = badHttpRequestException.Message }),

            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                new Dictionary<string, object?>
                {
                    ["errors"] = validationException.Errors
                        .GroupBy(failure => failure.PropertyName)
                        .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray())
                }),

            DomainValidationException domainValidationException => (
                StatusCodes.Status422UnprocessableEntity,
                domainValidationException.Message,
                new Dictionary<string, object?>()),

            DomainConflictException domainConflictException => (
                StatusCodes.Status409Conflict,
                domainConflictException.Message,
                new Dictionary<string, object?>()),

            DomainException domainException => (
                StatusCodes.Status400BadRequest,
                domainException.Message,
                new Dictionary<string, object?>()),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                new Dictionary<string, object?>())
        };
}
