# AMR-Core — Contexto para Claude Code

## Ecossistema
AMR-Core é o módulo core do **AMR SYSTEM** — ERP corporativo composto por 3 sistemas ativos:
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** (este repo) — SQLite, porta API :5001, web :5175
- **AMR-Forms-Fábrica** — SQLite, porta API :5186, web :5174

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQLite + Migrations
- Frontend: React 19 + TypeScript + Vite + Tailwind CSS + Lucide React
- Testes: xUnit + Coverlet (26 testes unitários)
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.Core.Domain/          # Entidades, value objects, enums, interfaces
├── AMR.Core.Application/     # CQRS handlers, DTOs, queries, commands
├── AMR.Core.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.Core.Shared/          # Result<T> e contratos compartilhados
└── AMR.Core.API/             # Controllers, Program.cs, DI
frontend/                      # React + Vite + TypeScript
tests/                         # xUnit
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI.

## Entidades do Domínio
- `Produto`, `UnidadeMedida`, `SaldoEstoque`, `MovimentoEstoque`
- `Fornecedor`, `Cliente` (com value objects `CNPJ`, `Endereco`)
- `PedidoCompra` + `ItemPedidoCompra` — workflow: Rascunho → Aprovado → Recebido
- `PedidoVenda` + `ItemPedidoVenda` — workflow: Rascunho → Aprovado → Faturado
- `Empresa`

## Comandos Principais
```bash
# Backend
cd src/AMR.Core.API && dotnet run
# → http://localhost:5001/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5175

# Testes
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.Core.Infrastructure --startup-project src/AMR.Core.API
dotnet ef database update --project src/AMR.Core.Infrastructure --startup-project src/AMR.Core.API
```

## Deploy AWS
Push para `main` dispara `deploy-aws.yml`:
1. Build Docker → push ECR (`amr-core-api`, `amr-core-web`)
2. Update ECS task definition + force deploy no cluster `amr-system`
3. Health check no ALB

- **Produção:** `amr-system-1908797477.sa-east-1.elb.amazonaws.com:8081`
- **Cluster:** `amr-system` | **Region:** `sa-east-1` | **Account:** `474874558993`
- **ECR:** `amr-core-api`, `amr-core-web`
- **EFS:** montado em `/data` para persistência do SQLite

## Estado do Projeto — Sprint 6 ativa (02/06–24/06/2026)
### Sprint 5 concluída (01/06/2026)
- Infra Terraform unificada provisionada na AWS
- CI/CD GitHub Actions funcionando para AMR-Core e AMR-Fábrica
- 13 testes unitários passando

### Sprint 6 — Documentação & Polish (02/06–24/06/2026)
**Concluído:**
- Formulários de criação com modais e dropdowns (Produto, Pedido de Compra, Pedido de Venda)
- Dashboard com KPIs de produtos, compras e vendas
- Endpoints GET para dropdowns (`/api/fornecedor`, `/api/cliente`, `/api/unidademedida`)
- Swagger XML docs — 6 controllers, 17 endpoints documentados
- 13 novos testes Application handlers (CriarProduto, AprovarPedidoCompra, FaturarPedidoVenda) → 26 total
- README revisado para .NET 10 / React 19
- CLAUDE.md criado no AMR-Core

**Pendente:**
- CLAUDE.md em AMR-Financeiro e AMR-Fábrica
- AMR-Fábrica re-deploy confirmado em produção
- Documentação final Notion consolidada

## Troubleshooting Frequente
| Problema | Solução |
|---|---|
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5001` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5175"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| Vite proxy não funciona | Atualizar `vite.config.ts` com a URL correta do backend |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
