# EduConnect

EduConnect é uma plataforma para controle e gestão de cursos, professores e alunos. Este é um projeto de estudos utilizado para testes de ferramentas e técnicas de desenvolvimento de software.

O sistema é composto por dois módulos principais: uma API REST no backend e uma interface web no frontend. Cada módulo tem sua própria documentação detalhada com informações de tecnologias, estrutura, execução e padrões de código.

---

## Pré-requisitos

Para trabalhar no projeto, instale:

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — necessário para o backend
- [Node.js 20+](https://nodejs.org/) com npm — necessário para o frontend
- [Docker](https://www.docker.com/) com Docker Compose — necessário para subir dependências locais de desenvolvimento

---

## Documentação por módulo

### [backend/README.md](backend/README.md)

Documentação da API REST. Inclui tecnologias utilizadas, estrutura de pastas, como executar a aplicação e os testes, endpoints disponíveis e os padrões de código adotados no projeto.

### [frontend/README.md](frontend/README.md)

Documentação da interface web. Inclui tecnologias utilizadas, estrutura de pastas, como executar em modo desenvolvimento e produção, lint e os padrões de código adotados no projeto.

---

## Ambiente de desenvolvimento

O diretório `dev-env/` concentra arquivos de suporte para o ambiente local de desenvolvimento. A ideia é manter dependências externas, como banco de dados, fora da aplicação e facilmente reproduzíveis por qualquer pessoa que trabalhe no projeto.

Atualmente esse diretório contém um `docker-compose.yml` para subir um banco PostgreSQL em container. Ele existe para:

- Padronizar a versão e a configuração inicial do banco usado em desenvolvimento.
- Evitar a necessidade de instalar PostgreSQL diretamente na máquina local.
- Facilitar a futura integração do backend com EF Core e migrations.
- Manter a infraestrutura local separada do domínio e das camadas da aplicação, permitindo trocar a tecnologia de persistência com menor impacto no código.

### PostgreSQL com Docker Compose

Para subir o banco de dados:

```bash
cd dev-env
docker compose up -d
```

Configuração disponível para desenvolvimento:

| Item | Valor |
|---|---|
| Host | `localhost` |
| Porta | `5432` |
| Banco | `educonnect` |
| Usuário | `admin` |
| Senha | `passwd` |

String de conexão sugerida para o backend:

```text
Host=localhost;Port=5432;Database=educonnect;Username=admin;Password=passwd
```

Para verificar o status do container:

```bash
cd dev-env
docker compose ps
```

Para parar o banco sem remover os dados:

```bash
cd dev-env
docker compose down
```

Os dados ficam persistidos em um volume Docker nomeado. Para remover o container e apagar também os dados locais do banco:

```bash
cd dev-env
docker compose down -v
```

---

## Execução rápida

Aqui temos instruções diretas para colocar a solução em execução. Para instruções mais detalhadas consulte a documentação específica de cada módulo em [Documentação por módulo](#documentação-por-módulo).

### Banco de dados

```bash
cd dev-env
docker compose up -d
```

### Backend

```bash
cd backend/src
dotnet run
```

API disponível em `http://localhost:5098`. Consulte [backend/README.md](backend/README.md) para mais opções.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Interface disponível em `http://localhost:3000`. Consulte [frontend/README.md](frontend/README.md) para mais opções.
