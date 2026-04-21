# Testes

## Visão geral

Atualmente a validação local é feita por:

- Build/lint do frontend
- Build das APIs .NET
- `go test`/`go vet` no Gateway
- Script E2E do fluxo Gateway + Wallet

## Frontend

```bash
cd web-app
npm run lint
npm run build
```

## Gateway (Go)

```bash
cd gateway-go
go test ./...
go vet ./...
```

## Wallet API / Quotes API (.NET)

```bash
cd services/wallet-api
dotnet build -c Debug

cd ..\\quotes-api
dotnet build -c Debug
```

## E2E (Gateway + Wallet)

O script abaixo executa:

- `POST /api/login`
- `GET /api/wallet/assets`
- `POST /api/wallet/assets/buy` (ticker novo)
- segunda compra no mesmo ticker (regressão importante)
- duas compras no ticker `PETR4`

```powershell
.\scripts\test-gateway-wallet.ps1
```

