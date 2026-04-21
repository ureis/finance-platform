# Variáveis de ambiente

Este projeto funciona em dev com defaults, mas algumas variáveis são esperadas para produção e/ou para padronizar ambientes.

## Gateway (`gateway-go`)

- **`JWT_SECRET`**: segredo HS256 para assinar/validar JWT.
  - Em dev pode ter default.
  - Em produção **deve** ser definido com um valor forte.

- **`GIN_MODE`**: `release`/`debug`
  - Em produção, `release`.

- **`GIN_TRUSTED_PROXIES`**: lista de proxies confiáveis (separado por vírgula)
  - Ex.: `10.0.0.0/8,192.168.0.0/16`

## Wallet API / Quotes API (.NET)

As APIs usam `appsettings.*` e variáveis para:

- Connection string do Postgres
- Configuração do RabbitMQ
- Logs/ambiente (Development/Production)

> Recomenda-se manter um `appsettings.Development.json` local e nunca commitar segredos.

## Frontend (`web-app`)

Em dev usa proxy do Vite:

- `/api` → `http://localhost:8080`
- `/ws` → `ws://localhost:8080`

Para produção, normalmente configura-se o reverse proxy (Nginx/Ingress) para apontar `/api` ao Gateway.

