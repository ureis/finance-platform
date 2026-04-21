# Observabilidade

## Objetivos

- Rastrear uma requisição ponta a ponta (frontend → gateway → API).
- Facilitar debug de erros (401/500, falhas de banco, mensageria).
- Padronizar logs sem vazar dados sensíveis.

## `X-Correlation-ID`

- Deve ser aceito na entrada e propagado na saída.
- Se ausente, deve ser gerado.
- Deve ser incluído em logs (idealmente como campo estruturado `CorrelationId`).

## Identidade do usuário

- O Gateway valida JWT e injeta `X-User-Id` para as APIs .NET.
- As APIs podem usar esse header para escopo de dados e auditoria.

## Boas práticas de log

- **Estruturado**: campos como `CorrelationId`, `GatewayUserId`, `RequestPath`, `StatusCode`, `LatencyMs`.
- **Sem segredos**: nunca logar JWT, passwords, connection strings.
- **Erros**: logar stacktrace apenas no backend; para o cliente retornar mensagens seguras.

## Debug local

Checklist:

- Confirmar `X-Correlation-ID` nos logs do Gateway e Wallet API para a mesma chamada.
- Em 401/403, confirmar se o frontend enviou `Authorization: Bearer`.
- Em 500, checar middleware de exceção (Wallet API) e logs do EF/mensageria.

