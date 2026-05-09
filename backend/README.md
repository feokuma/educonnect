# EduConnect — Backend

EduConnect é uma plataforma educacional que conecta alunos e professores. Este repositório contém a API REST do backend, responsável por expor os recursos da aplicação via HTTP.

---

## Tecnologias

| Tecnologia | Versão | Papel |
|---|---|---|
| .NET / ASP.NET Core | 10.0 | Framework web e runtime |
| C# | 13 | Linguagem principal |
| ASP.NET Core OpenAPI | 10.0 | Geração de documentação OpenAPI |
| xUnit | 2.9 | Framework de testes |
| Shouldly | 4.3 | Assertions fluentes nos testes |
| NSubstitute | 5.3 | Mocks nos testes unitários |
| Microsoft.AspNetCore.Mvc.Testing | 10.0 | Testes de integração com servidor in-process |
| Testcontainers.PostgreSql | 4.11 | PostgreSQL isolado para testes de integração |
| coverlet | 6.0 | Coleta de cobertura de testes |

---

## Estrutura do projeto

```
backend/
├── educonnect.sln                  # Solution raiz
├── src/                            # Código da aplicação
│   ├── Program.cs                  # Entry point e composição do pipeline
│   ├── educonnect.csproj
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Controllers/                # Controllers da API (camada de apresentação)
│   │   ├── HealthController.cs
│   │   └── UsersController.cs
│   ├── Application/
│   │   └── DTOs/                   # Data Transfer Objects de entrada e saída
│   │       └── CreateUserRequestDto.cs
│   └── Properties/
│       └── launchSettings.json     # Perfis de execução local
└── tests/
    ├── integration/                # Testes de integração (camada HTTP)
    │   ├── Setup/
    │   │   ├── IntegrationWebAppFactory.cs
    │   │   └── IntegrationTestCollection.cs
    │   └── Controllers/
    │       ├── HealthControllerTests.cs
    │       └── UsersControllerTests.cs
    └── unit/                       # Testes unitários
```

---

## Como executar a aplicação

Pré-requisito: .NET 10 SDK instalado.

```bash
cd backend/src
dotnet run
```

A API ficará disponível em:

- HTTP: `http://localhost:5098`
- HTTPS: `https://localhost:7283`

Para forçar um perfil específico:

```bash
dotnet run --launch-profile https
```

A documentação OpenAPI (JSON) é exposta em `http://localhost:5098/openapi/v1.json` quando o ambiente é `Development`.

---

## Como executar os testes

### Todos os testes da solution

```bash
cd backend
dotnet test
```

### Apenas testes de integração

```bash
dotnet test tests/integration/educonnect.integration.csproj
```

### Apenas testes unitários

```bash
dotnet test tests/unit/educonnect.unit.csproj
```

### Filtrar por nome de teste

```bash
dotnet test --filter "FullyQualifiedName~HealthControllerTests"
```

### Com cobertura de código

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Endpoints disponíveis

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Health check da aplicação |
| `POST` | `/users` | Cria um novo usuário |

### `GET /health`

Retorna o status da aplicação.

```json
{
  "status": "ok",
  "timestamp": "2026-05-07T12:00:00+00:00"
}
```

### `POST /users`

Corpo da requisição:

```json
{
  "name": "Jane Doe",
  "email": "jane.doe@example.com"
}
```

Resposta `201 Created` com header `Location: /users/{id}`:

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Jane Doe",
  "email": "jane.doe@example.com",
  "createdAt": "2026-05-07T12:00:00+00:00"
}
```

---

## Padrões adotados

### Organização em camadas

O projeto segue uma separação de responsabilidades em camadas, mesmo que ainda inicial:

- **Controllers** — Recebem requisições HTTP, delegam trabalho e retornam respostas. Não contêm lógica de negócio.
- **Application/DTOs** — Objetos de transferência de dados que representam contratos de entrada e saída da API. São declarados como `record` para imutabilidade e igualdade estrutural.

### Roteamento

As rotas são definidas diretamente nos controllers com `[Route("...")]` no nível da classe e atributos HTTP (`[HttpGet]`, `[HttpPost]`) nos métodos. Não há prefixo `api/` — as rotas são simples e expressivas (ex: `/users`, `/health`).

### Respostas HTTP

Os controllers retornam `IActionResult` e usam os helpers do `ControllerBase`:

- `Ok(payload)` → `200`
- `Created(location, payload)` → `201` com header `Location`

### Nullable reference types

O projeto tem `<Nullable>enable</Nullable>`, portanto todos os tipos de referência são não-nulos por padrão. Tipos anuláveis devem ser explicitamente marcados com `?`.

### Testes de integração

Os testes de integração usam `WebApplicationFactory<Program>` para subir a aplicação em memória e executar requisições HTTP reais via `HttpClient`. Isso garante que o pipeline completo do ASP.NET Core (middleware, roteamento, serialização, application services, repositórios e EF Core) seja exercitado.

- A factory é compartilhada entre testes do mesmo grupo via `ICollectionFixture`, evitando múltiplas inicializações desnecessárias.
- Os testes rodam com `ASPNETCORE_ENVIRONMENT=Test`.
- A factory sobe um PostgreSQL isolado com Testcontainers usando a imagem `postgres:17-alpine`.
- A connection string é sobrescrita pela factory para usar o banco `educonnect-test` dentro do container de teste.
- A factory recria o banco de teste e aplica as migrations antes da execução.
- Docker precisa estar disponível para rodar os testes de integração, mas não é necessário subir o `docker-compose.yml` manualmente.
- Respostas são deserializadas com `ReadFromJsonAsync<T>` para validação do contrato de resposta.

### Acesso ao `Program` nos testes

A classe `Program` gerada pelos top-level statements é interna por padrão. O projeto da aplicação usa `InternalsVisibleTo` no `.csproj` para permitir que os testes de integração referenciem `Program` como tipo genérico da `WebApplicationFactory`.
