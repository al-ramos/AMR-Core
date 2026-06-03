# AMR-Forms-Fábrica — Contexto para Claude Code

## Ecossistema
AMR-Forms-Fábrica é o módulo de gestão fabril do **AMR SYSTEM** — ERP corporativo composto por 3 sistemas ativos:
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** — SQLite, porta API :5001, web :5175
- **AMR-Forms-Fábrica** (este repo) — SQLite, porta API :5186, web :5174

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQLite + Migrations
- Frontend: React 18 + TypeScript + Vite + Tailwind CSS + Lucide React
- Testes: xUnit (4 testes unitários)
- Mensageria: MassTransit + RabbitMQ (integração com AMR-Financeiro)
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.Forms.Fabrica.Domain/          # Entidades, value objects, enums, interfaces
├── AMR.Forms.Fabrica.Application/     # CQRS handlers, DTOs, queries, commands
├── AMR.Forms.Fabrica.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.Forms.Fabrica.Shared/          # Result<T> e contratos compartilhados
└── AMR.Forms.Fabrica.API/             # Controllers, Program.cs, DI
frontend/                               # React + Vite + TypeScript
tests/                                  # xUnit
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI.

## Entidades do Domínio
- `Ficha` — ficha de processo fabril; saída dispara ContaPagar no AMR-Financeiro
- `Inspecao` — registro de inspeção de qualidade
- `OrdemReparo` — ordem de reparo de equipamentos/peças
- Integração: NF transmitida → `ContaReceber` no AMR-Financeiro via MassTransit/RabbitMQ (fail-safe)

## Comandos Principais
```bash
# Backend
cd src/AMR.Forms.Fabrica.API && dotnet run
# → http://localhost:5186/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5174

# Testes
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.Forms.Fabrica.Infrastructure --startup-project src/AMR.Forms.Fabrica.API
dotnet ef database update --project src/AMR.Forms.Fabrica.Infrastructure --startup-project src/AMR.Forms.Fabrica.API
```

## Deploy AWS
Push para `main` dispara `deploy-aws.yml`:
1. Build Docker → push ECR (`amr-fabrica-api`, `amr-fabrica-web`)
2. Update ECS task definition + force deploy no cluster `amr-system`
3. Health check no ALB

- **Produção:** `amr-system-1908797477.sa-east-1.elb.amazonaws.com:8082`
- **Cluster:** `amr-system` | **Region:** `sa-east-1` | **Account:** `474874558993`
- **ECR:** `amr-fabrica-api`, `amr-fabrica-web`
- **EFS:** montado em `/data` para persistência do SQLite
- **Re-deploy confirmado:** 02/06/2026 ✅

## Estado do Projeto — Sprint 6 ativo (02/06–24/06/2026)
- Entidades Ficha, Inspecao, OrdemReparo implementadas
- Integração Fábrica → Financeiro: saída de ficha gera ContaPagar; NF transmitida gera ContaReceber
- MassTransit + RabbitMQ configurado (fail-safe)
- 4 testes unitários passando
- Docker + nginx.conf corrigidos (porta 80, IP hardcoded removido)
- CI/CD GitHub Actions (`deploy-aws.yml`) ativo
- Re-deploy confirmado em produção ✅ (02/06/2026 — HTTP 200 nos 3 serviços)

## Protocolo de Encerramento de Card

Ao concluir qualquer card/tarefa, executar nesta ordem:

1. **Git** — commit descritivo + `git push -u origin <branch>`
2. **Notion card** — atualizar `Entrega` para a data real e adicionar referência do commit
3. **Kanban** — confirmar que `Status` está `✅ Concluído`
4. **CLAUDE.md** — atualizar seção `Estado do Projeto` se houve mudança relevante
5. **Próximo Card** — identificar e mover para `▶️ Em andamento`
6. **Merge para main** — garantir que o CLAUDE.md atualizado esteja em `main`

## Troubleshooting Frequente
| Problema | Solução |
|---|---|
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5186` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5174"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| nginx blank page | Conferir `nginx.conf` → porta 80 e `try_files` com React Router |
| RabbitMQ não conecta | Docker Desktop rodando com container `rabbitmq:management` |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
