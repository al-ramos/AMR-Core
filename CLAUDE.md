# AMR-Core — Contexto para Claude Code

## Ecossistema
AMR-Core é o módulo core do **AMR SYSTEM** — ERP corporativo composto por 3 sistemas ativos:
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** (este repo) — SQLite, porta API :5001, web :5175
- **AMR-Forms-Fábrica** — SQLite, porta API :5186, web :5174

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQLite + Migrations
- Frontend: React 18 + TypeScript + Vite + Tailwind CSS + Lucide React
- Testes: xUnit + Coverlet (13 testes unitários)
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

## Estado do Projeto — Sprint 6 ativo (02/06–24/06/2026)
- Infra Terraform unificada provisionada na AWS
- CI/CD GitHub Actions funcionando para AMR-Core e AMR-Fábrica
- 26 testes unitários passando (13 domain + 13 application handlers)
- **Sprint 6 concluído no AMR-Core:**
  - Formulários de criação: Novo Produto, Novo Pedido de Compra, Novo Pedido de Venda (modais com dropdowns)
  - Dashboard `/dashboard` com KPIs de produtos, compras e vendas (`DashboardPage.tsx`)
  - Endpoints GET /api/fornecedor, /api/cliente, /api/unidademedida (para dropdowns)
  - Swagger XML docs — 6 controllers documentados
  - README revisado para .NET 10 / React 19
- **Frontend — páginas implementadas:** `ProdutosPage`, `PedidosCompraPage`, `PedidosVendaPage`, `DashboardPage`

## Protocolo de Encerramento de Card

Ao concluir qualquer card/tarefa, executar nesta ordem:

1. **Git** — commit descritivo + `git push -u origin <branch>`
2. **Notion card** — atualizar `Entrega` para a data real e adicionar referência do commit no conteúdo da página
3. **Kanban** — confirmar que `Status` está `✅ Concluído` (ou avançar se necessário)
4. **CLAUDE.md** — atualizar seção `Estado do Projeto` se houve mudança relevante de contexto
5. **Próximo Card** — identificar o próximo card no Backlog, mover para `▶️ Em andamento` no Kanban/Notion e atualizar a seção `## Próximo Card` abaixo

---

## Protocolo de Encerramento de Sessão

Disparado quando o usuário disser **"encerrar sessão"** (ou "fechar sessão", "fim do dia", "encerrando").

Executar em ordem:

1. Consolidar todos os cards concluídos na sessão (título, commit, link Notion)
2. Confirmar o Próximo Card atualizado no `CLAUDE.md`
3. Criar **1 evento no Google Calendar** com:
   - Título: `AMR-Core ✅ Sessão DD/MM/YYYY`
   - Data/hora: agora (duração 30 min)
   - Descrição: cards entregues + commits + próximo card
   - Reminder: e-mail 0 minutos antes (notificação imediata)

> Apenas 1 chamada de Calendar por sessão — independente de quantos cards foram feitos.

---

## Próximo Card

**📝 Documentação final Notion — AMR Ecosystem consolidado**
- Sprint: 6 | Prioridade: Alta | Status: `▶️ Em andamento`
- Notion: https://www.notion.so/374d35f21de58191939acf6c08a6e3e5
- Itens:
  - [ ] CLAUDE.md em AMR-Financeiro
  - [ ] CLAUDE.md em AMR-Fábrica
  - [ ] AMR-Fábrica re-deploy confirmado em produção
  - [ ] Backlog Ágil — Sprint 6 atualizado com todos os entregáveis
  - [ ] AMR SYSTEM — Documentação Completa revisada

---

## Troubleshooting Frequente
| Problema | Solução |
|---|---|
| Porta errada no backend | Verificar `launchSettings.json` → `applicationUrl: http://localhost:5001` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: ["http://localhost:5175"]` |
| MediatR não resolve handlers | Usar native registration (`.AddMediatR(...)`), remover `MediatR.Extensions` |
| Vite proxy não funciona | Atualizar `vite.config.ts` com a URL correta do backend |
| AWS CLI no Git Bash | Prefixar com `MSYS_NO_PATHCONV=1 aws ecs ...` |
