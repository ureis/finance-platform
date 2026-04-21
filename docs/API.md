# API (Contratos e exemplos)

Este documento descreve os endpoints principais expostos para o frontend via **Gateway**.

> Observação: no ambiente de desenvolvimento, o frontend acessa tudo via `/api` (proxy do Vite → `http://localhost:8080`).

## Gateway (`http://localhost:8080`)

### Login (público)

- **POST** `/api/login`
- **Body**: `{}` (dev)
- **Response 200**:

```json
{ "token": "<jwt>" }
```

### Wallet (protegido por JWT)

Todas as rotas abaixo exigem:

- Header `Authorization: Bearer <jwt>`

#### Listar ativos

- **GET** `/api/wallet/assets`
- **Response 200**: lista de ativos

#### Comprar ativo

- **POST** `/api/wallet/assets/buy`
- **Body**:

```json
{
  "ticker": "PETR4",
  "name": "Petrobras",
  "type": "Stocks",
  "quantity": 10,
  "price": 20
}
```

#### Transações por ativo

- **GET** `/api/wallet/assets/{assetId}/transactions`
- **Response 200**: lista de transações

### WebSocket de preços

- **WS** `ws://localhost:8080/ws/prices`
- **Mensagem (exemplo)**:

```json
{ "ticker": "PETR4", "newPrice": 22.15 }
```

## Wallet API (`http://localhost:5212`)

O Gateway faz rewrite `/api/wallet/*` → `/v1/*` na Wallet API.

> Para uso via gateway, prefira sempre `http://localhost:8080/api/wallet/...`.

## Headers padrão

- **`X-Correlation-ID`**: rastreabilidade ponta a ponta.
- **`X-User-Id`**: fornecido pelo Gateway para a API (identidade do usuário).

## Códigos de erro (guideline)

- **401**: ausência/invalidade de JWT (Gateway)
- **403**: não autorizado (quando existir regra de autorização)
- **422**: validação de entrada (preferível para payload inválido)
- **500**: erro inesperado

