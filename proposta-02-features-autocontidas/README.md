# Proposta 02 — Features autocontidas com capacidades compartilhadas

Versão mínima e executável da alternativa **4.2** da RFC de estrutura arquitetural base do template .NET 10.
Existe para o time **visualizar** a proposta com código real, lado a lado com a
[proposta 03 — Boundaries físicos por responsabilidade](../proposta-03-boundaries-fisicos/).
As duas implementam **o mesmo exemplo** (`Products`: criar, buscar por id, descontinuar) e **os mesmos testes**, então a diferença entre elas é só a estrutura.

## A proposta

Esta alternativa é uma evolução da proposta anterior, mantendo a mesma divisão física em três projetos, mas introduz uma organização mais explícita das responsabilidades dentro de cada feature.

- O **Application** permanece como host e composition root da aplicação.
- O **Services** concentra as funcionalidades de negócio. Cada feature é tratada como uma unidade autocontida e organiza internamente seus casos de uso, modelo de domínio e implementações de infraestrutura. O **Commons**, dentro de Services, reúne elementos técnicos usados por todas as features no nível de persistência (ex.: Context e Migrations).
- O **Shared**, de nível mais alto, concentra building blocks e capacidades transversais que podem ser utilizados pelas diferentes features (behaviors, domain events, erros, guards, primitivos).

A proposta busca combinar duas propriedades: **coesão por funcionalidade**, mantendo tudo que pertence à feature próximo; e **separação lógica de responsabilidades**, distinguindo domínio, casos de uso e infraestrutura dentro de cada slice. Assim, o principal boundary físico continua sendo o projeto Services, enquanto os boundaries internos das features são definidos pela organização do código e pelas convenções arquiteturais.

## Estrutura implementada

```
src/
├── Sample.Application/                  host e composition root
│   ├── Configuration/
│   │   ├── IoC.cs                        Problem Details, JSON, Swagger
│   │   └── SwaggerExtensions.cs          Swagger só em Development (/swagger)
│   ├── Middleware/
│   │   └── GlobalExceptionHandler.cs     exceção → Problem Details (RFC 9457)
│   ├── appsettings*.json
│   └── Program.cs
├── Sample.Services/                     funcionalidades de negócio
│   ├── Commons/
│   │   ├── Context/SampleDbContext.cs   EF Core + Postgres
│   │   └── Migrations/
│   ├── Features/
│   │   └── Products/
│   │       ├── Domain/                   Product, ProductStatus, ProductGuard,
│   │       │                             ProductExceptions, IProductRepository
│   │       ├── Infrastructure/           ProductConfiguration, ProductRepository
│   │       ├── UseCases/
│   │       │   ├── Create/               Command, Handler, Validator, Response
│   │       │   ├── GetById/              Query, Handler, Response
│   │       │   └── Discontinue/          Command, Handler
│   │       └── Module.cs                 DI da feature + rotas HTTP
│   └── IoC.cs                            Mediator, validadores, DbContext, liga os Modules
└── Sample.Shared/                       capacidades transversais
    ├── Behaviors/ValidationBehavior.cs   valida Command/Query antes do handler
    ├── Errors/                           DomainException (400/422/409), ErrorOr → HTTP
    └── Modules/                          IModule + descoberta automática

test/
└── Sample.Tests.Unit/                   espelha src/Sample.Services
    └── Features/Products/
        ├── Domain/                       ProductTests + Fakers/
        └── UseCases/
            ├── Create/                   HandlerTests, ValidatorTests, Fakers/, Fixtures/, Mocks/
            ├── GetById/                  ...
            └── Discontinue/              ...
```

Dependências entre projetos (mão única):

```
Sample.Application ──► Sample.Services ──► Sample.Shared
```

### O `Module.cs` de cada feature

A feature se liga ao host pelo seu `Module.cs`, que implementa `IModule` (em `Shared/Modules`):

```csharp
public sealed class Module : IModule
{
    public void AddServices(IServiceCollection services)      // o que a feature registra no DI
    {
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    public void MapEndpoints(IEndpointRouteBuilder app)       // as rotas HTTP da feature
    {
        app.MapPost("/products", CreateAsync) ...
        app.MapGet("/products/{externalId:guid}", GetByIdAsync) ...
        app.MapPost("/products/{externalId:guid}/discontinuation", DiscontinueAsync) ...
    }
}
```

Implementar `IModule` já é o registro: `AddModules`/`MapModules` descobrem todos os módulos do assembly `Services` por reflection. Uma feature nova não exige alterar `Program.cs` nem nenhum outro arquivo fora da pasta dela.

### Caminho de uma requisição

```
POST /products
  → Module.CreateAsync                        (Features/Products/Module.cs)
  → Mediator → ValidationBehavior             (Shared/Behaviors)           entrada inválida → 400
  → CreateProductCommandHandler               (Features/Products/UseCases/Create)
  → Product.Create                            (Features/Products/Domain)   invariante violada → 422/409
  → IProductRepository → ProductRepository    (Features/Products/Infrastructure)
  → SampleDbContext                          (Commons/Context)
```

### Como adicionar uma feature

1. Criar `src/Sample.Services/Features/<Feature>/` com `Domain/`, `Infrastructure/` e `UseCases/<Verbo>/`.
2. Criar o `Module.cs` da feature implementando `IModule` (registro de DI + rotas).
3. Adicionar o `DbSet` no `SampleDbContext` e gerar a migration (ver abaixo).
4. Espelhar os testes em `test/Sample.Tests.Unit/Features/<Feature>/`.


## Trade-offs

| | |
|---|---|
| 🟢 | **Alta coesão por feature**: domínio, casos de uso e infraestrutura relacionados à mesma capacidade permanecem próximos. |
| 🟢 | **Melhor encapsulamento funcional**: cada feature possui uma estrutura própria e pode concentrar sua implementação sem espalhá-la por vários projetos. |
| 🟢 | **Separação conceitual mais clara**: Domain, UseCases e Infrastructure possuem locais explícitos dentro de cada feature. |
| 🟢 | **Baixa fragmentação física**: mantém poucos assemblies e reduz a navegação entre projetos. |
| 🟢 | **Capacidades transversais centralizadas**: mecanismos recorrentes podem ser padronizados através do Shared. |
| 🔴 | **Boundaries internos são lógicos, não físicos**: domínio, infraestrutura e exposição convivem no mesmo assembly; a estrutura de pastas comunica a arquitetura, mas o isolamento real depende de disciplina, revisão e guardrails adicionais (analyzers, testes arquiteturais). |
| 🔴 | **Acoplamento transitivo de dependências**: por residirem no mesmo assembly, testes e demais consumidores do domínio de uma feature acabam herdando pacotes de infraestrutura (ex.: EF Core, HTTP clients) mesmo sem precisar deles. |
| 🔴 | **Risco de dependências entre features**: por compartilharem o mesmo projeto Services, uma feature pode tecnicamente acessar elementos internos de outra se não houver mecanismos adicionais de isolamento. |
| 🔴 | **Shared assume papel estrutural relevante**: muitas features podem passar a depender dele, aumentando seu raio de impacto conforme novas capacidades forem adicionadas. |
| 🔴 | **Possível tensão em domínios mais ricos**: conceitos de domínio que atravessam diferentes casos de uso ou features exigem uma estratégia clara para evitar duplicação, dependências entre slices ou migração excessiva de conceitos para Shared. |
| 🔴 | **Menor expressividade do domínio na estrutura**: a organização interna é predominantemente técnica, o que pode dificultar a leitura do sistema a partir dos conceitos de negócio. |

### O que apareceu na prática ao montar esta versão

- **Acoplamento transitivo**: o projeto de testes unitários referencia `Sample.Services` para testar o domínioe os handlers, e por isso herda EF Core e Npgsql. Isso chegou a gerar um conflito de versão do `Microsoft.EntityFrameworkCore.Relational` no projeto de testes, resolvido fixando a versão em `Directory.Packages.props`. Na proposta 03 os testes referenciam só `Domain` e `Application`, e o problema não existe.
- **Shared depende de ASP.NET Core**: para oferecer `IModule` (rotas) e a tradução de `ErrorOr` para resposta HTTP, o `Sample.Shared` precisa de `FrameworkReference` para `Microsoft.AspNetCore.App`. Os building blocks de domínio (`DomainException`) convivem no mesmo assembly que esses elementos HTTP.
- **`internal` não protege nada dentro da feature**: `ProductRepository` e `ProductConfiguration` são `internal`, mas as rotas (`Module.cs`) e os casos de uso estão no mesmo assembly, então uma rota compila normalmente se injetar o repositório ou o `SampleDbContext` direto, pulando o caso de uso. Na proposta 03 o  mesmo `internal` já impede o endpoint (projeto WebApi) de enxergar o `ProductRepository`, embora o `SampleDbContext` público continue alcançável.

## Síntese

A alternativa preserva a **feature como principal boundary de organização**, adicionando separação interna entre domínio, casos de uso e infraestrutura e **centralizando capacidades transversais** em Shared. Em contrapartida, parte relevante dos boundaries permanece lógica, exigindo mecanismos adicionais para controlar as dependências dentro do assembly.
