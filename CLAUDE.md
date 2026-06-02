# AMR-Core — Contexto para Claude Code

## Este Repositório

**Sistema:** AMR-Core — ERP central (Produtos, Estoque, Compras, Clientes, Fornecedores)
**Stack backend:** .NET 8 · Clean Architecture · CQRS + MediatR · EF Core 8 · SQLite
**Stack frontend:** React 18 · Vite · TypeScript · Tailwind CSS
**Portas locais:** API `:5001` · Web `:5175`
**Banco dev:** SQLite — `src/AMR.Core.API/amr-core-dev.db`
**Banco prod:** SQLite montado em EFS (AWS ECS Fargate)

### Estrutura do projeto

```
src/
├── AMR.Core.Domain/          # Entidades, Value Objects, interfaces de repositório
├── AMR.Core.Application/     # CQRS Handlers, Commands, Queries, DTOs
├── AMR.Core.Infrastructure/  # EF Core, Migrations, Repositórios
├── AMR.Core.API/             # Controllers, DI, appsettings, Dockerfile
├── AMR.Core.Shared/          # Utilitários compartilhados
tests/
└── AMR.Core.Tests/           # 13 testes unitários (xUnit + Moq)
frontend/                     # React app (Vite + TypeScript + Tailwind)
```

### Entidades principais

- `Produto`, `Categoria`
- `Fornecedor`, `Cliente`
- `PedidoCompra` (workflow: Rascunho → Aprovado → Recebido)
- `MovimentacaoEstoque`
- `Empresa`

### Comandos essenciais

```bash
# Rodar API
dotnet run --project src/AMR.Core.API/AMR.Core.API.csproj
# Swagger: http://localhost:5001/swagger

# Rodar frontend
cd frontend && npm run dev
# Web: http://localhost:5175

# Build
dotnet build
dotnet clean && dotnet build

# Testes
dotnet test
dotnet test --verbosity normal
dotnet test /p:CollectCoverage=true /p:CoverletThreshold=40

# Migrations (EF Core)
dotnet ef migrations add <NomeDaMigracao> \
  --project src/AMR.Core.Infrastructure \
  --startup-project src/AMR.Core.API
dotnet ef database update \
  --project src/AMR.Core.Infrastructure \
  --startup-project src/AMR.Core.API

# Reset do banco (EF Core recria + seed automaticamente ao subir)
rm src/AMR.Core.API/amr-core-dev.db
```

### Issues conhecidos

- **Porta errada:** `launchSettings.json` → applicationUrl deve ser `:5001`
- **CORS bloqueando frontend:** `appsettings.Development.json` → AllowedOrigins deve incluir `http://localhost:5175`
- **npm install:** Sempre rodar após clonar (deps não ficam no repo)
- **recharts:** Manter versão 2.x — se v3 instalada: `npm install recharts@2.15.0`
- **Página em branco no frontend:** Verificar `main.tsx` — precisa ter `createRoot` (React 18)

---

## AMR Ecosystem — Contexto Geral

O AMR é um ecossistema ERP corporativo com três sistemas em produção:

| Sistema | Repo GitHub | API local | Web local | Banco |
|---|---|---|---|---|
| AMR-Core | `al-ramos/AMR-Core` | :5001 | :5175 | SQLite |
| AMR-Financeiro | `al-ramos/AMR-Financeiro` | :5015 | :5173 | SQL Server |
| AMR-Fábrica | `al-ramos/AMR-forms-fabrica` | :5186 | :5174 | SQLite |

### Stack comum

- **Backend:** .NET 8 · Clean Architecture · CQRS + MediatR 12 · EF Core 8 · FluentValidation · Serilog
- **Frontend:** React 18 · Vite · TypeScript · Tailwind CSS · Lucide React
- **Mensageria:** RabbitMQ + MassTransit (Financeiro produz, Core/Fábrica consomem)
- **Infra:** AWS ECS Fargate · ECR · ALB `amr-system` · EFS · Secrets Manager
- **IaC:** Terraform 1.5+ em `C:\GitHub\AMR\infra\terraform\`
- **CI/CD:** GitHub Actions — push para `main` dispara build → ECR → ECS force-deploy

### AWS (produção)

- **Cluster:** `amr-system` · Region: `sa-east-1` · Account: `426222909134`
- **ALB:** `amr-system-1908797477.sa-east-1.elb.amazonaws.com`
  - Financeiro → `:80` | Core → `:8081` | Fábrica → `:8082`
- **Créditos AWS:** $40 restantes — expiram **11/07/2026**
- **ECR repos (6):** `amr-core-api`, `amr-core-web`, `amr-financeiro-api`, `amr-financeiro-web`, `amr-fabrica-api`, `amr-fabrica-web`
- **EFS:** 3 volumes (um por sistema) para persistência SQLite
- **Secrets Manager:** `amr-financeiro/jwt-key`

### Scripts de automação (`C:\GitHub\AMR\automation\`)

```powershell
.\start-amr-dev.ps1 [-Only api|web|fin|fabrica|core]  # sobe serviços + RabbitMQ
.\stop-amr-dev.ps1                                      # para tudo
.\open-amr.ps1 [-Only web|api]                          # abre URLs no browser
.\git-amr.ps1 -m "mensagem" [-Only fin|fab|core]        # commit + push
.\aws-amr.ps1 [-Start|-Stop] [-Only Financeiro|Core|Fabrica]
.\monitor-deploy.ps1 [-Cluster amr-system -Service <nome>]
```

> Executar sempre a partir de `C:\GitHub` — rodar de outra pasta causa `CommandNotFoundException`.
> RabbitMQ sobe via Docker (`docker-compose up -d rabbitmq` em `AMR-Financeiro`). APIs e frontends sobem via `dotnet run` / `npm run dev`.

### Terraform (infra unificada)

```powershell
cd C:\GitHub\AMR\infra\terraform
.\apply.ps1              # init + plan + apply completo
.\apply.ps1 -PlanOnly    # só mostra o plano
.\apply.ps1 -Destroy     # destrói tudo (custo zero após demo)
```

### AWS — comandos avulsos

```bash
# Login no ECR
aws ecr get-login-password --region sa-east-1 | \
  docker login --username AWS --password-stdin 426222909134.dkr.ecr.sa-east-1.amazonaws.com

# Force-deploy de serviço específico
MSYS_NO_PATHCONV=1 aws ecs update-service \
  --cluster amr-system --service amr-core-api \
  --force-new-deployment --region sa-east-1

# Ver logs no CloudWatch
aws logs tail /ecs/amr-core/api --follow --region sa-east-1

# Stop/start containers (sem destruir infra)
for svc in amr-financeiro-api amr-financeiro-web amr-core-api amr-core-web amr-fabrica-api amr-fabrica-web; do
  MSYS_NO_PATHCONV=1 aws ecs update-service --region sa-east-1 --cluster amr-system --service $svc --desired-count 0
done
```

### Sprint atual

| Sprint | Período | Foco | Status |
|---|---|---|---|
| Sprint 5 | 28/05–10/06/2026 | Terraform + CI/CD + README | ⚡ Ativa |
| Sprint 6 | 11/06–24/06/2026 | Documentação final + polish | 🔜 Próxima |
| Release 1.0 | — | Junho/2026 | 🎯 Meta |

### Backlog técnico prioritário

1. Jest + RTL setup no frontend (cobertura frontend: 0% atualmente)
2. Cobertura backend: meta ≥40% (atualmente ~8%)
3. HTTPS via ACM + ALB
4. Logs centralizados (Serilog → CloudWatch)
5. Consolidar error handling das APIs (responses inconsistentes)
6. Rate limiting (AspNetCoreRateLimit)

### Convenções de código

- CQRS: Commands em `Application/Commands/`, Queries em `Application/Queries/`
- Handlers: `AddMediatR` nativo (sem `MediatR.Extensions` — removido no v12)
- Migrations: sempre especificar `--project` e `--startup-project` explicitamente
- Seed de dados demo já configurado — rodar após deletar o banco SQLite
- Login Financeiro (demo): `admin / admin123`
