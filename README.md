# 🔩 AMR-Core

> Módulo core do ecossistema AMR SYSTEM — gestão de produtos, fornecedores, clientes, estoque e pedidos de compra.

[![CI](https://github.com/alexsandro-ramos/AMR-Core/actions/workflows/ci.yml/badge.svg)](https://github.com/alexsandro-ramos/AMR-Core/actions/workflows/ci.yml)
[![Deploy AWS](https://github.com/alexsandro-ramos/AMR-Core/actions/workflows/deploy-aws.yml/badge.svg)](https://github.com/alexsandro-ramos/AMR-Core/actions/workflows/deploy-aws.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)

Parte do [AMR SYSTEM](../README.md) — veja a documentação do ecossistema completo.

---

## ✨ Funcionalidades

- **Produtos** — cadastro com categorização, unidade de medida e estoque mínimo
- **Fornecedores** — CRUD com CNPJ, contato e histórico de compras
- **Clientes** — gestão com CPF/CNPJ e endereço
- **Movimentação de Estoque** — entradas, saídas e saldo atual por produto
- **Pedidos de Compra** — workflow completo (Rascunho → Aprovado → Recebido)
- **Integração** — consome AMR-Financeiro para vinculação de notas fiscais

---

## 🛠️ Stack

| Camada | Tecnologia |
|---|---|
| Backend | .NET 8 + Clean Architecture + CQRS (MediatR) |
| ORM | EF Core 8 + SQLite |
| Frontend | React 18 + TypeScript + Vite + Tailwind CSS |
| Testes | xUnit + Coverlet |

---

## 🚀 Rodando localmente

```powershell
# Backend
cd src/AMR.Core.API
dotnet run
# → http://localhost:5001/swagger

# Frontend
cd frontend
npm install
npm run dev
# → http://localhost:5175
```

Ou suba o ecossistema completo com `.\automation\start-amr-dev.ps1` na raiz do AMR SYSTEM.

---

## 🏗️ Estrutura

```
src/
├── AMR.Core.Domain/          # Entidades, value objects, regras
├── AMR.Core.Application/     # CQRS handlers, DTOs, validadores
├── AMR.Core.Infrastructure/  # EF Core, SQLite, repositórios
├── AMR.Core.Shared/          # Contratos compartilhados
└── AMR.Core.API/             # Controllers, Program.cs, DI
frontend/                      # React + Vite + TypeScript
tests/                         # xUnit + Coverlet
```

---

## ☁️ Deploy

Push para `main` dispara `.github/workflows/deploy-aws.yml`:

1. **Build & Push** — imagens API e Web para ECR (`amr-core-api`, `amr-core-web`)
2. **Deploy ECS** — registra nova task definition + force new deployment no cluster `amr-system`
3. **Health check** — aguarda ALB responder na porta 8081

**AWS Secrets necessários:** `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`
