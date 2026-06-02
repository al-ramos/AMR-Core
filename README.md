# AMR-Core

Módulo core do **AMR SYSTEM** — gestão de produtos, estoque, pedidos de compra e pedidos de venda.

[![CI](https://github.com/al-ramos/amr-core/actions/workflows/ci.yml/badge.svg)](https://github.com/al-ramos/amr-core/actions/workflows/ci.yml)
[![Deploy AWS](https://github.com/al-ramos/amr-core/actions/workflows/deploy-aws.yml/badge.svg)](https://github.com/al-ramos/amr-core/actions/workflows/deploy-aws.yml)
![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![React 19](https://img.shields.io/badge/React-19-61DAFB?logo=react)

---

## Funcionalidades

| Módulo | Descrição |
|---|---|
| **Produtos** | Catálogo com SKU, unidade de medida, preço e estoque mínimo |
| **Fornecedores** | Cadastro com CNPJ e categoria |
| **Clientes** | PJ (CNPJ) e PF (CPF) |
| **Pedidos de Compra** | Workflow: Rascunho → Aprovado → Recebido |
| **Pedidos de Venda** | Workflow: Rascunho → Aprovado → Faturado (baixa estoque automática) |
| **Estoque** | Saldo por produto/empresa com histórico de movimentos |
| **Dashboard** | KPIs consolidados: produtos, pedidos e valor faturado |

---

## Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 10 + Clean Architecture + CQRS (MediatR 12) |
| ORM | EF Core 9 + SQLite + Migrations |
| Frontend | React 19 + TypeScript + Vite + Bootstrap 5 |
| State | TanStack React Query 5 |
| Testes | xUnit + Coverlet (domínio + application handlers) |
| Infra | AWS ECS Fargate + ECR + ALB + EFS |
| CI/CD | GitHub Actions |

---

## Arquitetura

```
src/
├── AMR.Core.Domain/          # Entidades, value objects, enums, interfaces
├── AMR.Core.Application/     # CQRS commands/queries/handlers, DTOs
├── AMR.Core.Infrastructure/  # EF Core, SQLite, repositórios, migrations
├── AMR.Core.Shared/          # Result<T> e contratos compartilhados
└── AMR.Core.API/             # Controllers, Program.cs, DI, Swagger
frontend/
└── src/
    ├── pages/                # ProdutosPage, PedidosCompraPage, PedidosVendaPage, DashboardPage
    └── api/                  # Clientes Axios por recurso
tests/
└── AMR.Core.Domain.Tests/    # Testes de domínio + handlers da camada Application
```

---

## Endpoints da API

Swagger disponível em `/swagger` ao rodar localmente.

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/produto` | Lista produtos ativos |
| `POST` | `/api/produto` | Cadastra produto |
| `GET` | `/api/fornecedor?empresaId=1` | Lista fornecedores ativos |
| `GET` | `/api/cliente?empresaId=1` | Lista clientes ativos |
| `GET` | `/api/unidademedida` | Lista unidades de medida |
| `GET` | `/api/pedidocompra?empresaId=1&status=Rascunho` | Lista pedidos de compra |
| `GET` | `/api/pedidocompra/{id}` | Detalhe do pedido de compra |
| `POST` | `/api/pedidocompra` | Cria pedido de compra |
| `PATCH` | `/api/pedidocompra/{id}/aprovar` | Aprova pedido de compra |
| `PATCH` | `/api/pedidocompra/{id}/receber` | Recebe pedido (atualiza estoque) |
| `GET` | `/api/pedidovenda?empresaId=1&status=Aprovado` | Lista pedidos de venda |
| `GET` | `/api/pedidovenda/{id}` | Detalhe do pedido de venda |
| `POST` | `/api/pedidovenda` | Cria pedido de venda |
| `PATCH` | `/api/pedidovenda/{id}/aprovar` | Aprova pedido de venda |
| `PATCH` | `/api/pedidovenda/{id}/faturar` | Fatura pedido (baixa estoque) |

---

## Rodando localmente

```bash
# Backend (http://localhost:5001/swagger)
cd src/AMR.Core.API
dotnet run

# Frontend (http://localhost:5175)
cd frontend
npm install
npm run dev

# Testes
dotnet test

# Nova migration
dotnet ef migrations add <Nome> \
  --project src/AMR.Core.Infrastructure \
  --startup-project src/AMR.Core.API

dotnet ef database update \
  --project src/AMR.Core.Infrastructure \
  --startup-project src/AMR.Core.API
```

---

## Deploy AWS

Push para `main` dispara `.github/workflows/deploy-aws.yml`:

1. **Build & Push** — imagens `amr-core-api` e `amr-core-web` para ECR
2. **Deploy ECS** — nova task definition + force deploy no cluster `amr-system`
3. **Health check** — aguarda ALB responder na porta `8081`

**Produção:** `amr-system-1908797477.sa-east-1.elb.amazonaws.com:8081`

**Secrets necessários no GitHub:** `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`
