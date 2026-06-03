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
│   ├── Interfaces/           # IProdutoRepository, IFornecedorRepository, IClienteRepository,
│   │                         #   IUnidadeMedidaRepository, IPedidoCompraRepository,
│   │                         #   IPedidoVendaRepository, ISaldoEstoqueRepository, IUnitOfWork
│   ├── Produtos/Commands/    # CriarProdutoCommand + CriarProdutoHandler
│   ├── Fornecedores/Queries/ # ListarFornecedoresQuery + Handler
│   ├── Clientes/Queries/     # ListarClientesQuery + Handler
│   ├── UnidadesMedida/       # ListarUnidadesMedidaQuery + Handler
│   ├── PedidosCompra/        # Criar, Aprovar, Receber + Listar (commands + queries)
│   ├── PedidosVenda/         # Criar, Aprovar, Faturar + Listar (commands + queries)
│   └── DTOs/                 # ProdutoDto, FornecedorDto, ClienteDto, UnidadeMedidaDto, ...
├── AMR.Core.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
│   └── Data/Repositories/    # ProdutoRepository, FornecedorRepository, ClienteRepository,
│                             #   UnidadeMedidaRepository, PedidoCompraRepository,
│                             #   PedidoVendaRepository, SaldoEstoqueRepository
├── AMR.Core.Shared/          # Result<T> e contratos compartilhados
└── AMR.Core.API/             # Controllers (6), Program.cs, DI, Swagger XML
    └── Controllers/          # ProdutoController, FornecedorController, ClienteController,
                              #   UnidadeMedidaController, PedidoCompraController,
                              #   PedidoVendaController
frontend/
└── src/
    ├── pages/                # ProdutosPage, PedidosCompraPage, PedidosVendaPage, DashboardPage
    └── api/                  # produtos, fornecedores, clientes, pedidos, unidadesMedida (Axios)
tests/
└── AMR.Core.Domain.Tests/
    ├── Domain/               # 13 testes de entidades e value objects
    └── Application/
        ├── Fakes/            # FakeProdutoRepository, FakePedidoCompraRepository,
        │                     #   FakePedidoVendaRepository, FakeSaldoEstoqueRepository,
        │                     #   FakeUnitOfWork
        ├── CriarProdutoHandlerTests.cs          (4 testes)
        ├── AprovarPedidoCompraHandlerTests.cs   (4 testes)
        └── FaturarPedidoVendaHandlerTests.cs    (5 testes)
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI.

## Entidades do Domínio
- `Produto`, `UnidadeMedida`, `SaldoEstoque`, `MovimentoEstoque`
- `Fornecedor`, `Cliente` (com value objects `CNPJ`, `Endereco`)
- `PedidoCompra` + `ItemPedidoCompra` — workflow: Rascunho → Aprovado → Recebido
- `PedidoVenda` + `ItemPedidoVenda` — workflow: Rascunho → Aprovado → Faturado
- `Empresa`

## Padrões de Teste
- Fakes in-memory (sem Moq, sem NSubstitute) em `tests/.../Application/Fakes/`
- `FakeUnitOfWork` expõe `CommitCount` para verificar número de commits
- `FakeSaldoEstoqueRepository` expõe `Movimentos` para verificar baixas de estoque
- IDs atribuídos via reflection (`typeof(Entidade).GetProperty("Id")!.SetValue(...)`)
- Todos os handlers testados isoladamente sem banco de dados

## Endpoints da API (15 total — 6 controllers)
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

## Estado do Projeto

### Sprint 5 ✅ concluída (01/06/2026)
- Infra Terraform unificada provisionada na AWS
- CI/CD GitHub Actions para AMR-Core e AMR-Fábrica
- 13 testes unitários de domínio passando

### Sprint 6 ⚡ (02/06–24/06/2026) — polish + documentação
Entregues em 02/06/2026 (adiantado):
- ✅ Formulários modais "Novo" em Produtos, Pedidos Compra e Pedidos Venda com dropdowns
- ✅ DashboardPage com KPIs de produtos, compras e vendas (sem novo endpoint — usa React Query)
- ✅ 3 novos endpoints GET para dropdowns: `/api/fornecedor`, `/api/cliente`, `/api/unidademedida`
- ✅ Swagger XML docs — 6 controllers, 15 endpoints documentados
- ✅ 13 novos testes application handlers → 26 testes total
- ✅ README revisado com badges .NET 10 / React 19 e tabela de endpoints
- ✅ CLAUDE.md criado e atualizado (este arquivo)

Pendente na Sprint 6:
- ⬜ CLAUDE.md em AMR-Financeiro e AMR-Fábrica
- ⬜ AMR-Fábrica re-deploy confirmado em produção
- ⬜ Documentação final Notion consolidada

## Troubleshooting Frequente
| Problema | Solução |
|---|---|
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5001` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5175"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| Vite proxy não funciona | Atualizar `vite.config.ts` com a URL correta do backend |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
