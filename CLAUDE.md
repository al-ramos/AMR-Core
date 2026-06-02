# AMR-Core — Contexto para Claude Code

## Ecossistema
AMR-Core é o módulo core do **AMR SYSTEM** — ERP corporativo composto por 3 sistemas ativos:
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** (este repo) — SQLite, porta API :5001, web :5175
- **AMR-Forms-Fábrica** — SQLite, porta API :5186, web :5174

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core 9 + SQLite + Migrations
- Frontend: React 19 + TypeScript + Vite + Bootstrap 5 + TanStack React Query 5
- Testes: xUnit + Coverlet (26 testes unitários: 13 domínio + 13 application handlers)
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.Core.Domain/          # Entidades, value objects, enums, interfaces
├── AMR.Core.Application/     # CQRS handlers, DTOs, queries, commands
├── AMR.Core.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.Core.Shared/          # Result<T> e contratos compartilhados
└── AMR.Core.API/             # Controllers, Program.cs, DI
frontend/
└── src/
    ├── pages/                # ProdutosPage, PedidosCompraPage, PedidosVendaPage, DashboardPage
    └── api/                  # Clientes Axios por recurso (produtos, fornecedores, clientes, pedidos, unidadesMedida)
tests/
└── AMR.Core.Domain.Tests/    # Testes de domínio + Application/Fakes + Application handler tests
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

## Estado do Projeto — Sprint 6 em andamento (02/06/2026)

### Sprint 5 ✅ concluída (01/06/2026)
- Infra Terraform unificada provisionada na AWS
- CI/CD GitHub Actions funcionando para AMR-Core e AMR-Fábrica
- 13 testes unitários de domínio passando

### Sprint 6 ⚡ (11/06–24/06/2026) — polish + documentação
Entregue em 02/06/2026 (adiantado):
- ✅ **Formulários completos**: modais "Novo" em Produtos, Pedidos Compra e Pedidos Venda com dropdowns de Fornecedor/Cliente/UnidadeMedida
- ✅ **Dashboard funcional**: KPIs de produtos, pedidos de compra e pedidos de venda (contagens + valor faturado)
- ✅ **Swagger com XML docs**: todos os controllers documentados com `<summary>` + `<response>`
- ✅ **26 testes unitários**: 13 domínio + 13 application handlers (CriarProduto, AprovarPedidoCompra, FaturarPedidoVenda)
- ✅ **README atualizado**: badges .NET 10/React 19, tabela completa de 15 endpoints, stack e arquitetura

### Endpoints da API (15 total)
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/produto` | Lista produtos ativos |
| POST | `/api/produto` | Cadastra produto |
| GET | `/api/fornecedor?empresaId=1` | Lista fornecedores ativos |
| GET | `/api/cliente?empresaId=1` | Lista clientes ativos |
| GET | `/api/unidademedida` | Lista unidades de medida |
| GET | `/api/pedidocompra?empresaId=1&status=Rascunho` | Lista pedidos de compra |
| GET | `/api/pedidocompra/{id}` | Detalhe do pedido de compra |
| POST | `/api/pedidocompra` | Cria pedido de compra |
| PATCH | `/api/pedidocompra/{id}/aprovar` | Aprova pedido de compra |
| PATCH | `/api/pedidocompra/{id}/receber` | Recebe pedido (atualiza estoque) |
| GET | `/api/pedidovenda?empresaId=1&status=Aprovado` | Lista pedidos de venda |
| GET | `/api/pedidovenda/{id}` | Detalhe do pedido de venda |
| POST | `/api/pedidovenda` | Cria pedido de venda |
| PATCH | `/api/pedidovenda/{id}/aprovar` | Aprova pedido de venda |
| PATCH | `/api/pedidovenda/{id}/faturar` | Fatura pedido (baixa estoque) |

## Troubleshooting Frequente
| Problema | Solução |
|---|---|
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5001` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5175"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| Vite proxy não funciona | Atualizar `vite.config.ts` com a URL correta do backend |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
