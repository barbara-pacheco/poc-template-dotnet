namespace Sample.WebApi._Shared;

/// <summary>
/// Contrato que cada endpoint HTTP implementa pra se registrar sozinho —
/// implementar a interface JÁ é o registro, sem precisar de uma chamada
/// manual em Program.cs. Ver EndpointExtensions, que descobre e mapeia
/// todo mundo que implementa isso.
/// </summary>
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
