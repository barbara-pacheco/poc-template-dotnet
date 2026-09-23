using ErrorOr;

namespace Sample.WebApi._Shared;

/// <summary>
/// Traduz uma falha ESPERADA (o handler devolveu ErrorOr com erro, não deu
/// throw) em resposta HTTP no formato Problem Details. Chamado explicitamente
/// pelo endpoint, no ponto onde ele olha o resultado — não pega exceção
/// nenhuma. Ver GlobalExceptionHandler pra falha inesperada.
/// </summary>
public static class ErrorOrExtensions
{
    public static IResult ToHttpResult(this List<Error> errors)
    {
        var first = errors[0];

        var (statusCode, title) = first.Type switch
        {
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ErrorType.Validation => (StatusCodes.Status422UnprocessableEntity, "Unprocessable Entity"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status400BadRequest, "Bad Request")
        };

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: first.Description,
            extensions: new Dictionary<string, object?> { ["errorCode"] = first.Code });
    }
}
