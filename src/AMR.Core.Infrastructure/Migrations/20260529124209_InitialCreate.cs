using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.Core.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPRESA",
                columns: table => new
                {
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NM_RAZAO_SOCIAL = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NM_FANTASIA = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NR_IE = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DS_EMAIL = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESA", x => x.CD_EMPRESA);
                });

            migrationBuilder.CreateTable(
                name: "UNIDADE_MEDIDA",
                columns: table => new
                {
                    CD_UNIDADE = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_SIGLA = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    DS_UNIDADE = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ST_ATIVO = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UNIDADE_MEDIDA", x => x.CD_UNIDADE);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTE",
                columns: table => new
                {
                    CD_CLIENTE = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NM_CLIENTE = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TP_DOCUMENTO = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    NR_DOCUMENTO = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    DS_EMAIL = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTE", x => x.CD_CLIENTE);
                    table.ForeignKey(
                        name: "FK_CLIENTE_EMPRESA_CD_EMPRESA",
                        column: x => x.CD_EMPRESA,
                        principalTable: "EMPRESA",
                        principalColumn: "CD_EMPRESA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORNECEDOR",
                columns: table => new
                {
                    CD_FORNECEDOR = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NM_RAZAO_SOCIAL = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NM_FANTASIA = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    NR_IE = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DS_CATEGORIA = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    NM_CONTATO = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORNECEDOR", x => x.CD_FORNECEDOR);
                    table.ForeignKey(
                        name: "FK_FORNECEDOR_EMPRESA_CD_EMPRESA",
                        column: x => x.CD_EMPRESA,
                        principalTable: "EMPRESA",
                        principalColumn: "CD_EMPRESA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO",
                columns: table => new
                {
                    CD_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_SKU = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NM_PRODUTO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DS_PRODUTO = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    QT_ESTOQUE_MINIMO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    CD_UNIDADE_MEDIDA = table.Column<int>(type: "INTEGER", nullable: false),
                    ST_ATIVO = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO", x => x.CD_PRODUTO);
                    table.ForeignKey(
                        name: "FK_PRODUTO_UNIDADE_MEDIDA_CD_UNIDADE_MEDIDA",
                        column: x => x.CD_UNIDADE_MEDIDA,
                        principalTable: "UNIDADE_MEDIDA",
                        principalColumn: "CD_UNIDADE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_VENDA",
                columns: table => new
                {
                    CD_PEDIDO_VENDA = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_CLIENTE = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_STATUS = table.Column<int>(type: "INTEGER", nullable: false),
                    DT_EMISSAO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_APROVACAO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DT_FATURAMENTO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DS_OBSERVACAO = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_VENDA", x => x.CD_PEDIDO_VENDA);
                    table.ForeignKey(
                        name: "FK_PEDIDO_VENDA_CLIENTE_CD_CLIENTE",
                        column: x => x.CD_CLIENTE,
                        principalTable: "CLIENTE",
                        principalColumn: "CD_CLIENTE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_COMPRA",
                columns: table => new
                {
                    CD_PEDIDO_COMPRA = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_FORNECEDOR = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_STATUS = table.Column<int>(type: "INTEGER", nullable: false),
                    DT_EMISSAO = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DT_APROVACAO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DT_RECEBIMENTO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DS_OBSERVACAO = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_COMPRA", x => x.CD_PEDIDO_COMPRA);
                    table.ForeignKey(
                        name: "FK_PEDIDO_COMPRA_FORNECEDOR_CD_FORNECEDOR",
                        column: x => x.CD_FORNECEDOR,
                        principalTable: "FORNECEDOR",
                        principalColumn: "CD_FORNECEDOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTO_ESTOQUE",
                columns: table => new
                {
                    CD_MOVIMENTO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_TIPO = table.Column<int>(type: "INTEGER", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    DS_ORIGEM = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    DT_MOVIMENTO = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTO_ESTOQUE", x => x.CD_MOVIMENTO);
                    table.ForeignKey(
                        name: "FK_MOVIMENTO_ESTOQUE_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALDO_ESTOQUE",
                columns: table => new
                {
                    CD_SALDO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_EMPRESA = table.Column<int>(type: "INTEGER", nullable: false),
                    QT_SALDO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALDO_ESTOQUE", x => x.CD_SALDO);
                    table.ForeignKey(
                        name: "FK_SALDO_ESTOQUE_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITEM_PEDIDO_VENDA",
                columns: table => new
                {
                    CD_ITEM = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_PEDIDO_VENDA = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    PC_DESCONTO = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    ItemPedidoVendaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEM_PEDIDO_VENDA", x => x.CD_ITEM);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_VENDA_PEDIDO_VENDA_ItemPedidoVendaId",
                        column: x => x.ItemPedidoVendaId,
                        principalTable: "PEDIDO_VENDA",
                        principalColumn: "CD_PEDIDO_VENDA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_VENDA_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITEM_PEDIDO_COMPRA",
                columns: table => new
                {
                    CD_ITEM = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CD_PEDIDO_COMPRA = table.Column<int>(type: "INTEGER", nullable: false),
                    CD_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    ItemPedidoCompraId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEM_PEDIDO_COMPRA", x => x.CD_ITEM);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_COMPRA_PEDIDO_COMPRA_ItemPedidoCompraId",
                        column: x => x.ItemPedidoCompraId,
                        principalTable: "PEDIDO_COMPRA",
                        principalColumn: "CD_PEDIDO_COMPRA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_COMPRA_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_CD_EMPRESA",
                table: "CLIENTE",
                column: "CD_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_FORNECEDOR_CD_EMPRESA",
                table: "FORNECEDOR",
                column: "CD_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_COMPRA_CD_PRODUTO",
                table: "ITEM_PEDIDO_COMPRA",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_COMPRA_ItemPedidoCompraId",
                table: "ITEM_PEDIDO_COMPRA",
                column: "ItemPedidoCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_VENDA_CD_PRODUTO",
                table: "ITEM_PEDIDO_VENDA",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_VENDA_ItemPedidoVendaId",
                table: "ITEM_PEDIDO_VENDA",
                column: "ItemPedidoVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_ESTOQUE_CD_PRODUTO",
                table: "MOVIMENTO_ESTOQUE",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDO_COMPRA_CD_FORNECEDOR",
                table: "PEDIDO_COMPRA",
                column: "CD_FORNECEDOR");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDO_VENDA_CD_CLIENTE",
                table: "PEDIDO_VENDA",
                column: "CD_CLIENTE");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_CD_SKU",
                table: "PRODUTO",
                column: "CD_SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_CD_UNIDADE_MEDIDA",
                table: "PRODUTO",
                column: "CD_UNIDADE_MEDIDA");

            migrationBuilder.CreateIndex(
                name: "IX_SALDO_ESTOQUE_CD_PRODUTO_CD_EMPRESA",
                table: "SALDO_ESTOQUE",
                columns: new[] { "CD_PRODUTO", "CD_EMPRESA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UNIDADE_MEDIDA_CD_SIGLA",
                table: "UNIDADE_MEDIDA",
                column: "CD_SIGLA",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ITEM_PEDIDO_COMPRA");

            migrationBuilder.DropTable(
                name: "ITEM_PEDIDO_VENDA");

            migrationBuilder.DropTable(
                name: "MOVIMENTO_ESTOQUE");

            migrationBuilder.DropTable(
                name: "SALDO_ESTOQUE");

            migrationBuilder.DropTable(
                name: "PEDIDO_COMPRA");

            migrationBuilder.DropTable(
                name: "PEDIDO_VENDA");

            migrationBuilder.DropTable(
                name: "PRODUTO");

            migrationBuilder.DropTable(
                name: "FORNECEDOR");

            migrationBuilder.DropTable(
                name: "CLIENTE");

            migrationBuilder.DropTable(
                name: "UNIDADE_MEDIDA");

            migrationBuilder.DropTable(
                name: "EMPRESA");
        }
    }
}
