# Proposta 03 — Boundaries físicos por responsabilidade com organização por feature

Versão mínima e executável da alternativa **4.3** da RFC de estrutura arquitetural base do template .NET 10.
Existe para o time **visualizar** a proposta com código real, lado a lado com a
[proposta 02 — Features autocontidas com capacidades compartilhadas](../proposta-02-features-autocontidas/).
As duas implementam **o mesmo exemplo** (`Products`: criar, buscar por id, descontinuar) e **os mesmos testes**, então a diferença entre elas é só a estrutura.

## A proposta

Esta alternativa separa fisicamente as principais responsabilidades da aplicação em projetos distintos. Cada `_Shared` concentra um tipo diferente de elemento transversal, de acordo com a responsabilidade da camada:

- **`Domain/_Shared`**: primitivos de modelagem (ex.: `Entity`, `AggregateRoot`, `ValueObject`), por exemplo guards de invariantes e a base de domain events, sem qualquer dependência externa.
- **`Application/_Shared`**: as portas usadas para inversão de dependência com a Infra (ex.: `IRepository<T>`, `IUnitOfWork`, `IClock`, `IEventPublisher`), behaviors de pipeline e erros de aplicação.
- **`Infrastructure/_Shared`**: implementações concretas reaproveitadas entre features, como o `DbContext`, as `Migrations` e repositórios genéricos que implementam as portas definidas em Application.
- **`WebApi/_Shared`**: middlewares, convenção de registro de endpoints e o formato padrão de resposta/erro da API.

A feature continua sendo a principal referência de organização, mas sua implementação é **distribuída** entre WebApi, Application, Domain e Infrastructure, de acordo com a responsabilidade de cada elemento.

Como essas áreas possuem responsabilidades e razões de mudança distintas, cada uma é representada por um **assembly separado**. Isso permite tornar explícita a direção das dependências e restringir, estruturalmente, relações que não deveriam existir.

Diferentemente das alternativas anteriores, nas quais domínio, casos de uso e infraestrutura podem coexistir no mesmo assembly, esta abordagem cria **boundaries físicos** entre essas responsabilidades. O domínio, por exemplo, pode evoluir sem depender de HTTP, persistência, mensageria ou outros detalhes tecnológicos.

A organização por feature é mantida dentro de cada projeto, buscando combinar coesão funcional com isolamento entre responsabilidades.

## Estrutura implementada

```
src/
├── Sample.Domain/                        regras de negócio, sem nenhum pacote externo
│   ├── _Shared/
│   │   └── DomainException.cs            base das exceções de domínio (400/422/409)
│   └── Products/                         Product, ProductStatus, ProductGuard,
│                                         ProductExceptions, IProductRepository
├── Sample.Application/                   casos de uso
│   ├── _Shared/
│   │   └── ValidationBehavior.cs         valida Command/Query antes do handler
│   ├── Products/
│   │   ├── Create/                       Command, Handler, Validator, Response
│   │   ├── GetById/                      Query, Handler, Response
│   │   └── Discontinue/                  Command, Handler
│   └── IoC.cs                            Mediator + validadores
├── Sample.Infrastructure/                persistência
│   ├── _Shared/
│   │   └── SampleDbContext.cs            EF Core + Postgres
│   ├── Migrations/
│   ├── Products/                         ProductConfiguration, ProductRepository
│   └── IoC.cs                            DbContext + repositórios
└── Sample.WebApi/                        host HTTP e composition root
    ├── _Shared/
    │   ├── IEndpoint.cs                  contrato de endpoint + descoberta automática
    │   ├── EndpointExtensions.cs
    │   ├── ErrorOrExtensions.cs          ErrorOr → Problem Details
    │   ├── GlobalExceptionHandler.cs     exceção → Problem Details (RFC 9457)
    │   └── SwaggerExtensions.cs          Swagger só em Development (/swagger)
    ├── Products/
    │   ├── Create/CreateProductEndpoint.cs
    │   ├── GetById/GetProductByIdEndpoint.cs
    │   └── Discontinue/DiscontinueProductEndpoint.cs
    ├── IoC.cs                            Problem Details, JSON, Swagger, endpoints
    └── Program.cs

test/
└── Sample.Tests.Unit/                    referencia só Domain e Application
    ├── Domain/Products/                  ProductTests + Fakers/
    └── Application/Products/
        ├── Create/                       HandlerTests, ValidatorTests, Fakers/, Fixtures/, Mocks/
        ├── GetById/                      ...
        └── Discontinue/                  ...
```

Dependências entre projetos (mão única, garantidas pelo compilador):

```
Sample.WebApi ──► Sample.Application ──► Sample.Domain
      │                   ▲                    ▲
      └──► Sample.Infrastructure ──────────────┘
```

- `Domain` não referencia nada.
- `Application` enxerga só `Domain`.
- `Infrastructure` enxerga `Domain` e `Application`, e implementa as portas deles.
- `WebApi` referencia `Application` e `Infrastructure`, porque é o composition root: é ele que liga tudo no `Program.cs`.

### Caminho de uma requisição

```
POST /products
  → CreateProductEndpoint                     (WebApi/Products/Create)
  → Mediator → ValidationBehavior             (Application/_Shared)        entrada inválida → 400
  → CreateProductCommandHandler               (Application/Products/Create)
  → Product.Create                            (Domain/Products)            invariante violada → 422/409
  → IProductRepository → ProductRepository    (Domain → Infrastructure/Products)
  → SampleDbContext                           (Infrastructure/_Shared)
```

A mesma feature (`Products`) aparece nos quatro projetos, sempre na pasta de mesmo nome.

### Como adicionar uma feature

1. `Sample.Domain/<Feature>/`: entidade, regras (guard e exceções) e a interface do repositório.
2. `Sample.Application/<Feature>/<Verbo>/`: Command/Query, Handler, Validator e Response de cada caso de uso.
3. `Sample.Infrastructure/<Feature>/`: `IEntityTypeConfiguration` e o repositório. Registrar o repositório no `IoC.cs`, adicionar o `DbSet` no `SampleDbContext` e gerar a migration.
4. `Sample.WebApi/<Feature>/<Verbo>/`: um endpoint por caso de uso, implementando `IEndpoint`. É descoberto sozinho, sem precisar mexer no `Program.cs`.
5. Espelhar os testes em `test/Sample.Tests.Unit/Domain/<Feature>/` e `test/Sample.Tests.Unit/Application/<Feature>/`.

## Trade-offs

| | |
|---|---|
| 🟢 | **Boundaries explícitos**: responsabilidades distintas possuem fronteiras físicas e contratos de dependência mais claros. |
| 🟢 | **Enforcement estrutural**: parte das regras arquiteturais pode ser garantida pelas referências entre projetos, reduzindo a dependência exclusiva de convenções. |
| 🟢 | **Isolamento do domínio**: regras e modelos de negócio podem evoluir sem dependência direta de mecanismos de exposição ou infraestrutura. |
| 🟢 | **Evolução tecnológica mais localizada**: persistência, mensageria, HTTP e outras capacidades podem evoluir dentro de seus respectivos boundaries. |
| 🟢 | **Melhor suporte a domínios mais ricos**: existe uma fronteira explícita para agregados, invariantes, objetos de valor e demais conceitos que podem participar de vários casos de uso. |
| 🟢 | **Mantém organização por funcionalidade**: a separação física não impede que cada projeto seja estruturado por feature. |
| 🟢 | **Maior expressividade e isolamento do domínio**: o domínio possui uma fronteira própria e pode ser organizado a partir de conceitos e regras de negócio, reduzindo interferência de detalhes técnicos e deixando mais claro o modelo que sustenta os casos de uso. |
| 🟢 | **Maior aderência às convenções do ecossistema .NET**: a separação entre apresentação, aplicação, domínio e infraestrutura é amplamente documentada nas referências arquiteturais da própria Microsoft para aplicações ASP.NET Core, tornando a estrutura mais familiar para profissionais da stack. |
| 🟢 | **Menor dependência de conhecimento específico do template corporativo**: a estrutura se aproxima de modelos já conhecidos externamente, reduzindo o quanto um desenvolvedor precisa aprender de convenções exclusivas da empresa antes de compreender a solução. |
| 🟢 | **Domínio protegido por padrão, não por exceção**: mesmo que nem todo serviço tenha domínio rico, o template já oferece um lugar claro e isolado para regras de negócio quando a complexidade surgir. |
| 🔴 | **Maior fragmentação**: compreender uma funcionalidade ponta a ponta pode exigir navegação entre WebApi, Application, Domain e Infrastructure. |
| 🔴 | **Maior cerimônia estrutural**: novos casos de uso podem exigir alterações em mais de um projeto mesmo quando a funcionalidade é simples. |
| 🔴 | **Mais assemblies e referências**: aumenta a quantidade de projetos e relações que precisam ser compreendidas e mantidas. |
| 🔴 | **Maior custo cognitivo inicial**: o desenvolvedor precisa compreender os papéis e as regras de dependência de cada boundary antes de contribuir. |
| 🔴 | **Exige cuidado com elementos compartilhados**: `_Shared` em cada projeto precisa ter propósito claro para não recriar, dentro das camadas, o mesmo problema de um Shared genérico. |

### O que apareceu na prática ao montar esta versão

- **Domínio sem nenhum pacote**: o `Sample.Domain.csproj` não tem nenhum `PackageReference`. Se alguém tentar usar EF Core ou HTTP dentro de uma entidade, o código não compila.
- **Testes sem infraestrutura**: o projeto de testes referencia só `Domain` e `Application` e não herda EF Core nem Npgsql. Na proposta 02, o mesmo projeto de testes herda esses pacotes e chegou a ter conflito de versão.
- **`internal` vale como barreira**: `ProductRepository` e `ProductConfiguration` são `internal` no `Infrastructure`. Por isso um endpoint no `WebApi` não consegue instanciar ou injetar o repositório concreto, e é obrigado a passar pelo caso de uso.
- **O composition root abre uma brecha**: o `WebApi` precisa referenciar o `Infrastructure` para ligar o DI no `Program.cs`, e o `SampleDbContext` é público. Um endpoint compila normalmente se injetar o `SampleDbContext` direto. Fechar essa brecha exige teste de arquitetura (ex.: ArchUnitNET), que ainda não está nesta versão.
- **Cerimônia visível**: a feature `Products` ocupa 19 arquivos espalhados pelos quatro projetos (5 no Domain, 9 no Application, 2 no Infrastructure e 3 no WebApi). Um caso de uso novo sobre uma entidade que já existe toca dois projetos: Application (Command, Handler, Validator e Response) e WebApi (o endpoint). Uma entidade nova toca os quatro, incluindo o `IoC.cs` e o `SampleDbContext` do Infrastructure.

## Síntese

A alternativa estabelece um **baseline arquitetural mais explícito e previsível** para os serviços corporativos, utilizando boundaries físicos para controlar dependências e reduzir o acoplamento entre negócio e detalhes tecnológicos. Embora introduza maior fragmentação inicial, essa complexidade é utilizada para reduzir ambiguidade, limitar o raio de impacto das mudanças e tornar a arquitetura mais sustentável conforme os serviços e o próprio template evoluem.
