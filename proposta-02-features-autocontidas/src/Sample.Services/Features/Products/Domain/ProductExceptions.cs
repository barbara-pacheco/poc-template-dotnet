using Sample.Shared.Errors;

namespace Sample.Services.Features.Products.Domain;

public sealed class InvalidProductException(string message) 
    : DomainValidationException(message);

public sealed class ProductAlreadyDiscontinuedException(Guid externalId)
    : DomainConflictException($"O produto {externalId} já foi descontinuado.");
