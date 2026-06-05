using AMR.Core.Domain.Entities;
using AMR.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AMR.Core.Infrastructure.Data;

/// <summary>
/// Seed de dados demo para o AMR-Core.
/// Popula: UnidadesMedida, Empresa, Clientes, Fornecedores, Produtos, PedidosVenda.
/// Idempotente — só insere se as tabelas estiverem vazias.
/// </summary>
public static class AmrCoreSeed
{
    public static async Task AplicarAsync(AmrCoreDbContext ctx)
    {
        // ── UnidadesMedida ────────────────────────────────────────────────────
        if (!await ctx.UnidadesMedida.AnyAsync())
        {
            ctx.UnidadesMedida.AddRange(
                UnidadeMedida.Criar("UN",  "Unidade"),
                UnidadeMedida.Criar("KG",  "Quilograma"),
                UnidadeMedida.Criar("L",   "Litro"),
                UnidadeMedida.Criar("M",   "Metro"),
                UnidadeMedida.Criar("M2",  "Metro Quadrado"),
                UnidadeMedida.Criar("CX",  "Caixa"),
                UnidadeMedida.Criar("PC",  "Peça")
            );
            await ctx.SaveChangesAsync();
        }

        var umUn = await ctx.UnidadesMedida.FirstAsync(u => u.Sigla == "UN");
        var umKg = await ctx.UnidadesMedida.FirstAsync(u => u.Sigla == "KG");
        var umL  = await ctx.UnidadesMedida.FirstAsync(u => u.Sigla == "L");
        var umCx = await ctx.UnidadesMedida.FirstAsync(u => u.Sigla == "CX");
        var umPc = await ctx.UnidadesMedida.FirstAsync(u => u.Sigla == "PC");

        // ── Empresa ───────────────────────────────────────────────────────────
        if (!await ctx.Empresas.AnyAsync())
        {
            ctx.Empresas.Add(Empresa.Criar(
                razaoSocial:       "AMR Indústria e Comércio Ltda",
                nomeFantasia:      "AMR System",
                cnpj:              CNPJ.Criar("11.222.333/0001-81"),
                inscricaoEstadual: "123.456.789.000",
                email:             "comercial@amrsystem.com.br",
                telefone:          "(11) 3000-0000"
            ));
            await ctx.SaveChangesAsync();
        }

        var empresa = await ctx.Empresas.FirstAsync();

        // ── Clientes ──────────────────────────────────────────────────────────
        if (!await ctx.Clientes.AnyAsync())
        {
            ctx.Clientes.AddRange(
                Cliente.CriarPJ(empresa.Id, "Metalúrgica São Paulo Ltda",      CNPJ.Criar("22.333.444/0001-81"), "compras@metalsp.com.br",   "(11) 3100-1000"),
                Cliente.CriarPJ(empresa.Id, "Distribuidora Norte Sul S.A.",    CNPJ.Criar("33.444.555/0001-81"), "pedidos@nortesul.com.br",  "(21) 3200-2000"),
                Cliente.CriarPJ(empresa.Id, "Construtora Alfa Engenharia Ltda",CNPJ.Criar("44.555.666/0001-81"), "suprimentos@alfa.eng.br",  "(31) 3300-3000"),
                Cliente.CriarPF(empresa.Id, "Carlos Eduardo Martins",          "748.597.840-27",                  "carlos.martins@email.com", "(11) 99100-0001"),
                Cliente.CriarPF(empresa.Id, "Ana Paula Ferreira",              "539.818.200-87",                  "ana.ferreira@email.com",   "(11) 99200-0002")
            );
            await ctx.SaveChangesAsync();
        }

        // ── Fornecedores ──────────────────────────────────────────────────────
        if (!await ctx.Fornecedores.AnyAsync())
        {
            ctx.Fornecedores.AddRange(
                Fornecedor.Criar(empresa.Id, "Aço Rápido Matérias-Primas Ltda",  "Aço Rápido",  CNPJ.Criar("55.666.777/0001-81"), "Matéria-Prima", email: "vendas@acprapido.com.br",   telefone: "(11) 3400-4000"),
                Fornecedor.Criar(empresa.Id, "Embalagens Total Ltda",             "Total Emb",   CNPJ.Criar("66.777.888/0001-81"), "Embalagem",     email: "atendimento@totalemb.com.br", telefone: "(11) 3500-5000"),
                Fornecedor.Criar(empresa.Id, "Logística Expressa Transportes SA", "Log Express", CNPJ.Criar("77.888.999/0001-81"), "Transportadora", email: "frete@logexpress.com.br",   telefone: "(11) 3600-6000")
            );
            await ctx.SaveChangesAsync();
        }

        // ── Produtos ──────────────────────────────────────────────────────────
        if (!await ctx.Produtos.AnyAsync())
        {
            ctx.Produtos.AddRange(
                Produto.Criar("PARAF-M6-30",  "Parafuso M6x30 Inox",         precoUnitario: 0.85m,   unidadeMedidaId: umUn.Id, descricao: "Parafuso inox 304, M6x30mm, cabeça sextavada",      estoqueMinimo: 500),
                Produto.Criar("PARAF-M8-50",  "Parafuso M8x50 Zincado",      precoUnitario: 1.20m,   unidadeMedidaId: umUn.Id, descricao: "Parafuso zincado M8x50mm",                           estoqueMinimo: 300),
                Produto.Criar("PORCA-M6",     "Porca Sextavada M6 Inox",     precoUnitario: 0.45m,   unidadeMedidaId: umUn.Id, descricao: "Porca sextavada inox 304, M6",                       estoqueMinimo: 500),
                Produto.Criar("ARR-M6",       "Arruela Lisa M6",             precoUnitario: 0.18m,   unidadeMedidaId: umUn.Id, descricao: "Arruela lisa M6 zincada",                            estoqueMinimo: 1000),
                Produto.Criar("CHAPA-ACO-2",  "Chapa de Aço 2mm (m²)",       precoUnitario: 45.00m,  unidadeMedidaId: umKg.Id, descricao: "Chapa de aço carbono ABNT 1020, 2mm",                estoqueMinimo: 50),
                Produto.Criar("TUBO-RED-25",  "Tubo Redondo 25mm",           precoUnitario: 28.50m,  unidadeMedidaId: umKg.Id, descricao: "Tubo redondo aço carbono 25mm x 1,5mm",              estoqueMinimo: 30),
                Produto.Criar("TINTA-EPOXI-1","Tinta Epóxi 1L Cinza",        precoUnitario: 89.90m,  unidadeMedidaId: umL.Id,  descricao: "Tinta epóxi bicomponente, cor cinza médio, 1 litro", estoqueMinimo: 20),
                Produto.Criar("KIT-MANUT-HID","Kit Manutenção Hidráulica",   precoUnitario: 320.00m, unidadeMedidaId: umCx.Id, descricao: "Kit com vedações, mangueiras e conexões hidráulicas", estoqueMinimo: 5),
                Produto.Criar("ROLAM-6205",   "Rolamento 6205 2RS",          precoUnitario: 18.90m,  unidadeMedidaId: umPc.Id, descricao: "Rolamento rígido de esferas 6205 2RS, 25x52x15mm",   estoqueMinimo: 20),
                Produto.Criar("CORREIA-A50",  "Correia em V Perfil A-50",    precoUnitario: 32.00m,  unidadeMedidaId: umPc.Id, descricao: "Correia em V perfil A-50, borracha reforçada",        estoqueMinimo: 10)
            );
            await ctx.SaveChangesAsync();
        }

        // ── Pedidos de Venda ──────────────────────────────────────────────────
        if (!await ctx.PedidosVenda.AnyAsync())
        {
            var clientes  = await ctx.Clientes.ToListAsync();
            var produtos  = await ctx.Produtos.ToListAsync();
            var c1 = clientes[0]; var c2 = clientes[1]; var c3 = clientes[2];
            var p  = produtos.ToDictionary(p => p.SKU);

            // PV-001 — Faturado
            var pv1 = PedidoVenda.Criar(empresa.Id, c1.Id, "Entrega urgente — obra SP");
            pv1.AdicionarItem(p["PARAF-M6-30"].Id,  200, p["PARAF-M6-30"].PrecoUnitario);
            pv1.AdicionarItem(p["PORCA-M6"].Id,     200, p["PORCA-M6"].PrecoUnitario);
            pv1.AdicionarItem(p["ARR-M6"].Id,       400, p["ARR-M6"].PrecoUnitario);
            pv1.AdicionarItem(p["ROLAM-6205"].Id,    10, p["ROLAM-6205"].PrecoUnitario);
            pv1.Aprovar();
            pv1.Faturar();
            ctx.PedidosVenda.Add(pv1);

            // PV-002 — Aprovado
            var pv2 = PedidoVenda.Criar(empresa.Id, c2.Id, "Reposição de estoque mensal");
            pv2.AdicionarItem(p["CHAPA-ACO-2"].Id,   30, p["CHAPA-ACO-2"].PrecoUnitario);
            pv2.AdicionarItem(p["TUBO-RED-25"].Id,   15, p["TUBO-RED-25"].PrecoUnitario);
            pv2.AdicionarItem(p["TINTA-EPOXI-1"].Id,  8, p["TINTA-EPOXI-1"].PrecoUnitario);
            pv2.Aprovar();
            ctx.PedidosVenda.Add(pv2);

            // PV-003 — Aberto
            var pv3 = PedidoVenda.Criar(empresa.Id, c3.Id, "Orçamento para aprovação");
            pv3.AdicionarItem(p["KIT-MANUT-HID"].Id,  3, p["KIT-MANUT-HID"].PrecoUnitario);
            pv3.AdicionarItem(p["CORREIA-A50"].Id,    5, p["CORREIA-A50"].PrecoUnitario);
            pv3.AdicionarItem(p["PARAF-M8-50"].Id,  100, p["PARAF-M8-50"].PrecoUnitario);
            ctx.PedidosVenda.Add(pv3);

            // PV-004 — Cancelado
            var pv4 = PedidoVenda.Criar(empresa.Id, c1.Id, "Pedido cancelado pelo cliente");
            pv4.AdicionarItem(p["ROLAM-6205"].Id, 5, p["ROLAM-6205"].PrecoUnitario);
            pv4.Cancelar();
            ctx.PedidosVenda.Add(pv4);

            await ctx.SaveChangesAsync();
        }

        // ── Pedidos de Compra ─────────────────────────────────────────────────
        if (!await ctx.PedidosCompra.AnyAsync())
        {
            var fornecedores = await ctx.Fornecedores.ToListAsync();
            var produtos     = await ctx.Produtos.ToListAsync();
            var f1 = fornecedores[0]; // Aço Rápido
            var f2 = fornecedores[1]; // Total Emb
            var p  = produtos.ToDictionary(p => p.SKU);

            // PC-001 — Recebido
            var pc1 = PedidoCompra.Criar(empresa.Id, f1.Id, "Reposição estoque — matéria-prima");
            pc1.AdicionarItem(p["CHAPA-ACO-2"].Id,  50, p["CHAPA-ACO-2"].PrecoUnitario);
            pc1.AdicionarItem(p["TUBO-RED-25"].Id,  20, p["TUBO-RED-25"].PrecoUnitario);
            pc1.AdicionarItem(p["PARAF-M6-30"].Id, 500, p["PARAF-M6-30"].PrecoUnitario);
            pc1.Aprovar();
            pc1.Receber();
            ctx.PedidosCompra.Add(pc1);

            // PC-002 — Aprovado (aguardando entrega)
            var pc2 = PedidoCompra.Criar(empresa.Id, f1.Id, "Compra mensal parafusos e arruelas");
            pc2.AdicionarItem(p["PARAF-M8-50"].Id, 300, p["PARAF-M8-50"].PrecoUnitario);
            pc2.AdicionarItem(p["PORCA-M6"].Id,    300, p["PORCA-M6"].PrecoUnitario);
            pc2.AdicionarItem(p["ARR-M6"].Id,      600, p["ARR-M6"].PrecoUnitario);
            pc2.Aprovar();
            ctx.PedidosCompra.Add(pc2);

            // PC-003 — Aberto (rascunho)
            var pc3 = PedidoCompra.Criar(empresa.Id, f2.Id, "Embalagens para expedição Q3");
            pc3.AdicionarItem(p["TINTA-EPOXI-1"].Id, 10, p["TINTA-EPOXI-1"].PrecoUnitario);
            pc3.AdicionarItem(p["KIT-MANUT-HID"].Id,  2, p["KIT-MANUT-HID"].PrecoUnitario);
            ctx.PedidosCompra.Add(pc3);

            // PC-004 — Aberto
            var pc4 = PedidoCompra.Criar(empresa.Id, f1.Id, "Rolamentos e correias — manutenção");
            pc4.AdicionarItem(p["ROLAM-6205"].Id,  15, p["ROLAM-6205"].PrecoUnitario);
            pc4.AdicionarItem(p["CORREIA-A50"].Id,  8, p["CORREIA-A50"].PrecoUnitario);
            ctx.PedidosCompra.Add(pc4);

            await ctx.SaveChangesAsync();
        }

        // ── Ordens de Recebimento ─────────────────────────────────────────────
        if (!await ctx.OrdensRecebimento.AnyAsync())
        {
            var pcs = await ctx.PedidosCompra
                .Include(p => p.Itens)
                .ToListAsync();

            // OR-001 — Concluído (para PC-001 que já está Recebido)
            var pc1 = pcs.FirstOrDefault(p => p.Status == AMR.Core.Domain.Enums.StatusPedidoCompra.Recebido);
            if (pc1 is not null)
            {
                var or1 = OrdemRecebimento.Criar(pc1.Id, pc1.Itens.Select(i => (i.ProdutoId, i.Quantidade)));
                or1.IniciarRecebimento();
                foreach (var item in or1.Itens)
                    item.Receber(item.QntEsperada);
                or1.Concluir();
                ctx.OrdensRecebimento.Add(or1);
            }

            await ctx.SaveChangesAsync();
        }
    }
}
