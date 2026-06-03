# AMR-Financeiro — Contexto para Claude Code

## Ecossistema
AMR-Financeiro é o módulo financeiro do **AMR SYSTEM** — ERP corporativo composto por 3 sistemas ativos:
- **AMR-Financeiro** (este repo) — SQL Server, porta API :5015, web :5173
- **AMR-Core** — SQLite, porta API :5001, web :5175
- **AMR-Forms-Fábrica** — SQLite, porta API :5186, web :5174

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQL Server + Migrations
- Frontend: React 18 + TypeScript + Vite + Tailwind CSS + Lucide React
- Testes: xUnit (15 testes unitários)
- Segurança: JWT Bearer + PBKDF2/SHA256
- Mensageria: MassTransit + RabbitMQ
- Infra: AWS ECS Fargate + ECR + ALB + EFS + Secrets Manager | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.Financeiro.Domain/          # Entidades, value objects, enums, interfaces
├── AMR.Financeiro.Application/     # CQRS handlers, DTOs, queries, commands
├── AMR.Financeiro.Infrastructure/  # EF Core, SQL Server, repositories, UoW, migrations
├── AMR.Financeiro.Shared/          # Result<T> e contratos compartilhados
└── AMR.Financeiro.API/             # Controllers, Program.cs, DI
frontend/                            # React + Vite + TypeScript
tests/                               # xUnit
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI.

## Entidades do Domínio
- `PlanoContas` — hierarquia de contas contábeis
- `LancamentoFinanceiro` — débitos e créditos vinculados ao plano de contas
- `ContaPagar` — contas a pagar com baixa manual e geração de lançamento de débito
- `ContaReceber` — contas a receber com baixa e geração de lançamento de crédito
- `NotaFiscal` — integração com AMR-Fábrica (NF transmitida → ContaReceber via MassTransit)

## Comandos Principais
```bash
# Backend
cd src/AMR.Financeiro.API && dotnet run
# → http://localhost:5015/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5173

# Testes
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.Financeiro.Infrastructure --startup-project src/AMR.Financeiro.API
dotnet ef database update --project src/AMR.Financeiro.Infrastructure --startup-project src/AMR.Financeiro.API
```

## Deploy AWS
Push para `main` dispara `deploy-aws.yml`:
1. Build Docker → push ECR (`amr-financeiro-api`, `amr-financeiro-web`)
2. Update ECS task definition + force deploy no cluster `amr-system`
3. Health check no ALB

- **Produção:** `amr-system-1908797477.sa-east-1.elb.amazonaws.com:80`
- **Cluster:** `amr-system` | **Region:** `sa-east-1` | **Account:** `474874558993`
- **ECR:** `amr-financeiro-api`, `amr-financeiro-web`
- **Secrets Manager:** `amr-financeiro/jwt-key`

## Estado do Projeto — Sprint 6 ativo (02/06–24/06/2026)
- Domínio financeiro completo: PlanoContas, LancamentoFinanceiro, ContaPagar, ContaReceber
- JWT Authentication: seed admin, PBKDF2/SHA256, Bearer no Swagger
- MassTransit + RabbitMQ: integração Fábrica → Financeiro (saída de ficha → ContaPagar; NF → ContaReceber)
- Serilog + Health Checks configurados
- Docker + docker-compose funcionando
- CI/CD GitHub Actions (`deploy-aws.yml`) ativo
- 15 testes unitários passando

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
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5015` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5173"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| JWT inválido em produção | Verificar `Secrets Manager` → `amr-financeiro/jwt-key` |
| RabbitMQ não conecta | Docker Desktop rodando com container `rabbitmq:management` |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
