# poc-template-dotnet

Provas de conceito das alternativas de estrutura discutidas na
[RFC-0003 — Estrutura arquitetural base para o template corporativo .NET 10](https://nstech-empresa.atlassian.net/wiki/spaces/NPC/pages/221184009/RFC-0003+Estrutura+arquitetural+base+para+o+template+corporativo+.NET+10).

As propostas implementam o mesmo exemplo (`Products`: criar, buscar por id, descontinuar) com os mesmos testes. A única diferença entre elas é a estrutura.

| Proposta | Alternativa da RFC | Resumo |
|---|---|---|
| [proposta-02-features-autocontidas](proposta-02-features-autocontidas/) | 4.2 | Application (host), Services (features autocontidas) e Shared (capacidades transversais) |
| [proposta-03-boundaries-fisicos](proposta-03-boundaries-fisicos/) | 4.3 | Um assembly por responsabilidade (WebApi, Application, Domain, Infrastructure), organizado por feature |


## Quando optar por cada uma

Nenhuma das duas é melhor em absoluto. A pergunta que decide é **onde as regras de dependência devem ser garantidas**.

### Optar pela 02 quando

- os serviços gerados pelo template tendem a ser **pequenos ou médios**, com um time só como dono;
- o domínio é **majoritariamente CRUD com regras pontuais**, sem conceitos compartilhados por vários casos de uso;
- **velocidade de mudança** pesa mais que isolamento: criar, alterar e remover uma feature mexe numa pasta só;
- o time está disposto a manter **testes de arquitetura como parte obrigatória do template**. Sem eles, os boundaries da 02 existem só na estrutura de pastas e se desgastam com o tempo.

### Optar pela 03 quando

- o serviço deve viver muito tempo, **crescer em features** ou passar por vários times;
- existe (ou é provável que surja) **domínio rico**: agregados, invariantes, objetos de valor usados por vários casos de uso;
- o time prefere que o **compilador impeça** a violação (Domain sem nenhum pacote, `internal` valendo entre projetos) a depender de revisão e testes;
- **familiaridade com o ecossistema .NET** importa: a separação em camadas é a que a documentação da Microsoft e a maior parte dos profissionais da stack já conhecem.

### O que pesa contra cada uma

- **Contra a 02**: os problemas que ela tem já apareceram nesta PoC pequena, não são hipotéticos. O teste herdou EF Core e teve conflito de versão, o `Shared` depende de ASP.NET Core e o `internal` não impede uma rota de pular o caso de uso. Num serviço maior, isso tende a piorar.
- **Contra a 03**: a cerimônia aparece em todo serviço, inclusive nos simples. `Products` já ocupa 19 arquivos em 4 projetos, e uma entidade nova passa pelos quatro. Ela também não fecha tudo sozinha: o `SampleDbContext` continua alcançável pelo `WebApi` e precisa de teste de arquitetura assim como a 02.

### Em resumo

Se o template for a base de **qualquer** serviço da empresa, inclusive os que crescem, a 03 é a escolha mais segura: o custo dela é previsível e aparece no primeiro dia, enquanto o custo da 02 aparece tarde e é mais caro de reverter. Se o template for voltado a **microsserviços pequenos e focados**, a 02 com testes de arquitetura entrega a maior parte da proteção com bem menos atrito.
