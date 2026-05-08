# EduConnect

EduConnect é uma plataforma para controle e gestão de cursos, professores e alunos. Este é um projeto de estudos utilizado para testes de ferramentas e técnicas de desenvolvimento de software.

O sistema é composto por dois módulos principais: uma API REST no backend e uma interface web no frontend. Cada módulo tem sua própria documentação detalhada com informações de tecnologias, estrutura, execução e padrões de código.

---

## Pré-requisitos

Para trabalhar no projeto, instale:

- [.NET 10 SDK](https://dotnet.microsoft.com/download) — necessário para o backend
- [Node.js 20+](https://nodejs.org/) com npm — necessário para o frontend

---

## Documentação por módulo

### [backend/README.md](backend/README.md)

Documentação da API REST. Inclui tecnologias utilizadas, estrutura de pastas, como executar a aplicação e os testes, endpoints disponíveis e os padrões de código adotados no projeto.

### [frontend/README.md](frontend/README.md)

Documentação da interface web. Inclui tecnologias utilizadas, estrutura de pastas, como executar em modo desenvolvimento e produção, lint e os padrões de código adotados no projeto.

---

## Execução rápida

Aqui temos instruções diretas para colocar a solução em execução. Para isntruções mais detalhadas consulte a documentação especifica de cada módulo em [Documentação por módulo](#documentação-por-módulo)

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
