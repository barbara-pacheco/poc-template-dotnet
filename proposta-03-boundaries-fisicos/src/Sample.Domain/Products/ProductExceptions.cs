using Sample.Domain._Shared;

namespace Sample.Domain.Products;

public sealed class InvalidProductException(string message) 
    : DomainValidationException(message);

public sealed class ProductAlreadyDiscontinuedException(Guid externalId)
    : DomainConflictException($"O produto {externalId} já foi descontinuado.");
