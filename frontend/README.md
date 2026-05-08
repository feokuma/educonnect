# EduConnect — Frontend

EduConnect é uma plataforma educacional que conecta alunos e professores. Este repositório contém a interface web do produto, construída com Next.js e servida ao browser.

---

## Tecnologias

| Tecnologia | Versão | Papel |
|---|---|---|
| Next.js | 16 | Framework React com App Router, SSR e RSC |
| React | 19 | Biblioteca de UI |
| TypeScript | 5 | Linguagem principal |
| Tailwind CSS | 4 | Estilização utilitária |
| ESLint | 9 | Linting com regras Next.js + TypeScript |

---

## Estrutura do projeto

```
frontend/
├── app/                    # App Router do Next.js
│   ├── layout.tsx          # Layout raiz (fontes, metadata, html/body)
│   ├── page.tsx            # Página inicial (rota "/")
│   └── globals.css         # Estilos globais e tokens de tema Tailwind
├── public/                 # Arquivos estáticos servidos na raiz
├── types/                  # Tipos gerados automaticamente pelo Next.js
│   ├── routes.d.ts         # Mapa de rotas tipadas
│   ├── cache-life.d.ts     # Perfis de cache ("use cache")
│   └── validator.ts        # Validação de tipos de pages e layouts
├── next.config.ts          # Configuração do Next.js
├── tsconfig.json           # Configuração do TypeScript
├── eslint.config.mjs       # Configuração do ESLint (flat config)
├── postcss.config.mjs      # Configuração do PostCSS (Tailwind v4)
└── package.json
```

> Os arquivos dentro de `types/` são gerados automaticamente pelo Next.js. Não os edite manualmente.

---

## Como executar a aplicação

Pré-requisito: Node.js 20+ e npm instalados.

### Instalar dependências

```bash
cd frontend
npm install
```

### Modo desenvolvimento (com hot reload)

```bash
npm run dev
```

A aplicação ficará disponível em `http://localhost:3000`.

### Build de produção

```bash
npm run build
npm start
```

---

## Lint

```bash
npm run lint
```

O ESLint está configurado com as regras `next/core-web-vitals` e `next/typescript`, cobrindo boas práticas de performance e type-safety específicas do Next.js.

---

## Padrões adotados

### App Router

O projeto usa o **App Router** do Next.js (diretório `app/`), não o Pages Router. Toda nova página deve ser criada como um arquivo `page.tsx` dentro de uma pasta que representa o segmento de rota.

### React Server Components por padrão

Todos os componentes dentro de `app/` são **React Server Components (RSC)** por padrão. Para usar estado, efeitos ou APIs de browser, adicione `"use client"` no topo do arquivo para tornar o componente um Client Component.

### Estilização com Tailwind CSS v4

A estilização é feita exclusivamente com classes utilitárias do Tailwind CSS. A versão 4 usa a importação `@import "tailwindcss"` em vez do `@tailwind` directives da v3. Tokens de tema customizados são definidos em `globals.css` via `@theme`.

### Fontes

As fontes **Geist Sans** e **Geist Mono** são carregadas via `next/font/google` no `layout.tsx` e expostas como variáveis CSS (`--font-geist-sans`, `--font-geist-mono`), utilizadas pelo Tailwind.

### TypeScript estrito

O `tsconfig.json` tem `"strict": true`, o que ativa todas as verificações estritas do TypeScript. Tipos `any` explícitos devem ser evitados.

### Alias de importação

O alias `@/*` aponta para a raiz do projeto (`./`), permitindo importações absolutas como `import { X } from "@/app/components/X"` em vez de caminhos relativos longos.

### Configuração do ESLint

A configuração usa o formato **flat config** (`eslint.config.mjs`) introduzido no ESLint 9. As regras estendem `eslint-config-next/core-web-vitals` e `eslint-config-next/typescript`.
