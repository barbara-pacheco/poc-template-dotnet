namespace Sample.Shared.Errors;

/// <summary>
/// Violação de invariante do domínio, lançada pela própria entidade (via Guard).
/// Lançada direto como DomainException vira 400; os subtipos abaixo dizem ao
/// cliente o que houve (422 ou 409). "Não encontrado" nunca é exceção: é
/// ErrorOr devolvido pelo handler.
/// </summary>
public abstract class DomainException(string message) : Exception(message);

/// <summary>
/// Subtipo de DomainException que a API expõe como erro do cliente (422),
/// em vez de erro interno genérico (400).
/// </summary>
public abstract class DomainValidationException(string message) : DomainException(message);

/// <summary>
/// Subtipo de DomainException que a API expõe como 409: o pedido bate de
/// frente com o estado em que o recurso JÁ está (ex.: descontinuar o que já
/// foi descontinuado). Regra que só recusa o pedido é DomainValidationException (422).
/// </summary>
public abstract class DomainConflictException(string message) : DomainException(message);
