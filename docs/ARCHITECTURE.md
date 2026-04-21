# Arquitetura

Este documento descreve a arquitetura da **Finance Platform** (frontend + gateway + APIs .NET) e as decisões que guiam a evolução do código.

## Componentes

- **`web-app/`**: React + TypeScript + Vite (UI).
- **`gateway-go/`**: Go + Gin (Gateway).
  - Autenticação JWT (`POST /api/login`).
  - Rotas protegidas (`/api/**`) exigem `Authorization: Bearer <token>`.
  - Reverse proxy com rewrite: `/api/wallet/*` → Wallet API `/v1/*`.
  - WebSocket: `/ws/prices` para atualização em tempo real.
- **`services/wallet-api/`**: Wallet API (.NET, Minimal APIs, Clean Architecture).
- **`services/quotes-api/`**: Quotes API (.NET, Minimal APIs, Clean Architecture).
- **`infra/`**: Docker Compose com PostgreSQL + RabbitMQ.

## Fluxos principais

### 1) Autenticação

1. O frontend chama `POST /api/login` no Gateway.
2. O Gateway emite um JWT HS256.
3. O frontend armazena o token e envia `Authorization: Bearer ...` em chamadas subsequentes (`/api/**`).

### 2) Wallet via Gateway (proxy)

1. O frontend chama `GET /api/wallet/assets`.
2. O Gateway valida o JWT e faz proxy para a Wallet API em `http://localhost:5212/v1/assets`.
3. O Gateway propaga headers importantes:
   - `X-Correlation-ID` (rastreabilidade)
   - `X-User-Id` (identidade para as APIs .NET)

### 3) Preços em tempo real (WebSocket)

1. O frontend conecta em `ws://localhost:8080/ws/prices`.
2. O Gateway mantém um Hub de conexões e repassa updates recebidos (ex.: via RabbitMQ consumer).

## Clean Architecture (APIs .NET)

Organização por camadas:

- **Domain**: entidades, value objects (ex.: `Money`), invariantes e regras.
- **Application**: casos de uso, validação, DTOs e orquestração.
- **Infrastructure**: EF Core, Dapper, mensageria e integrações.
- **WebApi**: endpoints Minimal APIs, middlewares e composição (DI).

### Regras

- **Domínio não depende** de Infrastructure/WebApi.
- **Use cases** não devem conhecer HTTP.
- **Exceções** não são fluxo normal de negócio (preferir `Result`).

## Persistência (PostgreSQL)

- **Comandos**: EF Core (tracking e consistência).
- **Consultas**: Dapper quando houver necessidade de performance/projeções.
- **Migrations**: controlam versão do schema.
- Padrões:
  - tabelas em `snake_case` e plural
  - dinheiro com `decimal` e `precision/scale`

## Observabilidade

- `X-Correlation-ID` deve existir em toda requisição (gerado se ausente) e ser logado.
- Logs estruturados são preferíveis para rastrear fluxo ponta a ponta.

## Decisões importantes (ADR resumido)

- **JWT no Gateway**: centraliza autenticação/autorização para chamadas HTTP do frontend.
- **Reverse proxy**: reduz acoplamento do frontend com múltiplas APIs e padroniza headers.
- **Clean Architecture** nas APIs .NET: isola domínio e torna mudanças mais seguras.

