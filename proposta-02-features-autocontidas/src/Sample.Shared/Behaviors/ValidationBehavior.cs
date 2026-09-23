using FluentValidation;
using Mediator;

namespace Sample.Shared.Behaviors;

/// <summary>
/// Passo do pipeline do Mediator que roda os validadores do FluentValidation
/// de um Command/Query antes de deixar o handler executar. Entrada inválida
/// lança ValidationException e nunca chega ao handler.
/// </summary>
public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(message, cancellationToken);

        var failures = (await Task.WhenAll(
                validators.Select(validator => validator.ValidateAsync(message, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(message, cancellationToken);
    }
}
