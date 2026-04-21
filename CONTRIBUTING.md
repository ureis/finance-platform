# Contribuindo

## Regras gerais

- Mantenha mudanças pequenas e revisáveis.
- Não commite segredos (`.env`, tokens, connection strings).
- Antes de abrir PR, valide localmente.

## Checklist local

### Frontend

```bash
cd web-app
npm run lint
npm run build
```

### Gateway

```bash
cd gateway-go
go test ./...
go vet ./...
```

### APIs .NET

```bash
cd services/wallet-api
dotnet build -c Debug
dotnet format --verify-no-changes

cd ..\\quotes-api
dotnet build -c Debug
```

## Estilo e padrões

- Consulte `.cursorrules` para padrões de arquitetura, logs, Result pattern, etc.

