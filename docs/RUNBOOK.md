# Runbook (Operação e Troubleshooting)

Guia rápido para rodar localmente e resolver os problemas mais comuns.

## Subir tudo (ordem recomendada)

1. Infra (Postgres + RabbitMQ)
2. Wallet API
3. Gateway
4. Frontend

## Diagnósticos rápidos

### 401 no Dashboard

1. Recarregue a página e teste novamente.
2. Se persistir, limpe o token e gere outro:

```js
localStorage.removeItem("token")
location.reload()
```

3. Confirme que o Gateway está rodando em `:8080`.

### Porta ocupada

- Wallet API: `5212`
- Gateway: `8080`
- Postgres: `5432`
- RabbitMQ: `5672`/`15672`

Feche processos antigos ou altere a porta do serviço.

### Wallet API não inicia / migrations

- Verifique se o Postgres está no ar (`docker ps`).
- Logs da Wallet API indicam se há migrations pendentes.

### RabbitMQ fora do ar

- Abra o painel: `http://localhost:15672` (guest/guest).
- Verifique se o container `finance-messenger` está rodando.

### WebSocket não conecta

1. Confirme o Gateway em `:8080`.
2. O frontend usa `ws://localhost:8080/ws/prices`.
3. Verifique se o browser não está bloqueando por mixed content (HTTPS vs WS).

## Comandos úteis

### Infra

```bash
docker compose -f infra/docker-compose.yml up -d
docker compose -f infra/docker-compose.yml ps
docker compose -f infra/docker-compose.yml logs -f
```

### Wallet API

```bash
cd services/wallet-api/Wallet.Api
dotnet run --urls http://localhost:5212
```

### Gateway

```bash
cd gateway-go
go run ./cmd/gateway
```

### Frontend

```bash
cd web-app
npm install
npm run dev
```

