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
            migrationBuilder.EnsureSchema(
                name: "erp");

            migrationBuilder.CreateTable(
                name: "EMPRESA",
                schema: "erp",
                columns: table => new
                {
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NM_RAZAO_SOCIAL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NM_FANTASIA = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    NR_IE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DS_EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPRESA", x => x.CD_EMPRESA);
                });

            migrationBuilder.CreateTable(
                name: "UNIDADE_MEDIDA",
                schema: "erp",
                columns: table => new
                {
                    CD_UNIDADE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_SIGLA = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DS_UNIDADE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ST_ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UNIDADE_MEDIDA", x => x.CD_UNIDADE);
                });

            migrationBuilder.CreateTable(
                name: "CLIENTE",
                schema: "erp",
                columns: table => new
                {
                    CD_CLIENTE = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NM_CLIENTE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TP_DOCUMENTO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    NR_DOCUMENTO = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    DS_EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENTE", x => x.CD_CLIENTE);
                    table.ForeignKey(
                        name: "FK_CLIENTE_EMPRESA_CD_EMPRESA",
                        column: x => x.CD_EMPRESA,
                        principalSchema: "erp",
                        principalTable: "EMPRESA",
                        principalColumn: "CD_EMPRESA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FORNECEDOR",
                schema: "erp",
                columns: table => new
                {
                    CD_FORNECEDOR = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NM_RAZAO_SOCIAL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NM_FANTASIA = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NR_CNPJ = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    NR_IE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DS_CATEGORIA = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    NR_TELEFONE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NM_CONTATO = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DS_LOGRADOURO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NR_NUMERO = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_COMPLEMENTO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_BAIRRO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_CIDADE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DS_ESTADO = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    NR_CEP = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    DS_PAIS = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ST_ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FORNECEDOR", x => x.CD_FORNECEDOR);
                    table.ForeignKey(
                        name: "FK_FORNECEDOR_EMPRESA_CD_EMPRESA",
                        column: x => x.CD_EMPRESA,
                        principalSchema: "erp",
                        principalTable: "EMPRESA",
                        principalColumn: "CD_EMPRESA",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTO",
                schema: "erp",
                columns: table => new
                {
                    CD_PRODUTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NM_PRODUTO = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DS_PRODUTO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    QT_ESTOQUE_MINIMO = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CD_UNIDADE_MEDIDA = table.Column<int>(type: "int", nullable: false),
                    ST_ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO", x => x.CD_PRODUTO);
                    table.ForeignKey(
                        name: "FK_PRODUTO_UNIDADE_MEDIDA_CD_UNIDADE_MEDIDA",
                        column: x => x.CD_UNIDADE_MEDIDA,
                        principalSchema: "erp",
                        principalTable: "UNIDADE_MEDIDA",
                        principalColumn: "CD_UNIDADE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_VENDA",
                schema: "erp",
                columns: table => new
                {
                    CD_PEDIDO_VENDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false),
                    CD_CLIENTE = table.Column<int>(type: "int", nullable: false),
                    CD_STATUS = table.Column<int>(type: "int", nullable: false),
                    DT_EMISSAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_APROVACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DT_FATURAMENTO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_OBSERVACAO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_VENDA", x => x.CD_PEDIDO_VENDA);
                    table.ForeignKey(
                        name: "FK_PEDIDO_VENDA_CLIENTE_CD_CLIENTE",
                        column: x => x.CD_CLIENTE,
                        principalSchema: "erp",
                        principalTable: "CLIENTE",
                        principalColumn: "CD_CLIENTE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_COMPRA",
                schema: "erp",
                columns: table => new
                {
                    CD_PEDIDO_COMPRA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false),
                    CD_FORNECEDOR = table.Column<int>(type: "int", nullable: false),
                    CD_STATUS = table.Column<int>(type: "int", nullable: false),
                    DT_EMISSAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DT_APROVACAO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DT_RECEBIMENTO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_OBSERVACAO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_COMPRA", x => x.CD_PEDIDO_COMPRA);
                    table.ForeignKey(
                        name: "FK_PEDIDO_COMPRA_FORNECEDOR_CD_FORNECEDOR",
                        column: x => x.CD_FORNECEDOR,
                        principalSchema: "erp",
                        principalTable: "FORNECEDOR",
                        principalColumn: "CD_FORNECEDOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTO_ESTOQUE",
                schema: "erp",
                columns: table => new
                {
                    CD_MOVIMENTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_PRODUTO = table.Column<int>(type: "int", nullable: false),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false),
                    CD_TIPO = table.Column<int>(type: "int", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DS_ORIGEM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DT_MOVIMENTO = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTO_ESTOQUE", x => x.CD_MOVIMENTO);
                    table.ForeignKey(
                        name: "FK_MOVIMENTO_ESTOQUE_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalSchema: "erp",
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALDO_ESTOQUE",
                schema: "erp",
                columns: table => new
                {
                    CD_SALDO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_PRODUTO = table.Column<int>(type: "int", nullable: false),
                    CD_EMPRESA = table.Column<int>(type: "int", nullable: false),
                    QT_SALDO = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALDO_ESTOQUE", x => x.CD_SALDO);
                    table.ForeignKey(
                        name: "FK_SALDO_ESTOQUE_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalSchema: "erp",
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITEM_PEDIDO_VENDA",
                schema: "erp",
                columns: table => new
                {
                    CD_ITEM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_PEDIDO_VENDA = table.Column<int>(type: "int", nullable: false),
                    CD_PRODUTO = table.Column<int>(type: "int", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    PC_DESCONTO = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ItemPedidoVendaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEM_PEDIDO_VENDA", x => x.CD_ITEM);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_VENDA_PEDIDO_VENDA_ItemPedidoVendaId",
                        column: x => x.ItemPedidoVendaId,
                        principalSchema: "erp",
                        principalTable: "PEDIDO_VENDA",
                        principalColumn: "CD_PEDIDO_VENDA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_VENDA_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalSchema: "erp",
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ITEM_PEDIDO_COMPRA",
                schema: "erp",
                columns: table => new
                {
                    CD_ITEM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CD_PEDIDO_COMPRA = table.Column<int>(type: "int", nullable: false),
                    CD_PRODUTO = table.Column<int>(type: "int", nullable: false),
                    QT_QUANTIDADE = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    VL_PRECO_UNITARIO = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ItemPedidoCompraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITEM_PEDIDO_COMPRA", x => x.CD_ITEM);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_COMPRA_PEDIDO_COMPRA_ItemPedidoCompraId",
                        column: x => x.ItemPedidoCompraId,
                        principalSchema: "erp",
                        principalTable: "PEDIDO_COMPRA",
                        principalColumn: "CD_PEDIDO_COMPRA",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ITEM_PEDIDO_COMPRA_PRODUTO_CD_PRODUTO",
                        column: x => x.CD_PRODUTO,
                        principalSchema: "erp",
                        principalTable: "PRODUTO",
                        principalColumn: "CD_PRODUTO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLIENTE_CD_EMPRESA",
                schema: "erp",
                table: "CLIENTE",
                column: "CD_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_FORNECEDOR_CD_EMPRESA",
                schema: "erp",
                table: "FORNECEDOR",
                column: "CD_EMPRESA");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_COMPRA_CD_PRODUTO",
                schema: "erp",
                table: "ITEM_PEDIDO_COMPRA",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_COMPRA_ItemPedidoCompraId",
                schema: "erp",
                table: "ITEM_PEDIDO_COMPRA",
                column: "ItemPedidoCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_VENDA_CD_PRODUTO",
                schema: "erp",
                table: "ITEM_PEDIDO_VENDA",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_ITEM_PEDIDO_VENDA_ItemPedidoVendaId",
                schema: "erp",
                table: "ITEM_PEDIDO_VENDA",
                column: "ItemPedidoVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_ESTOQUE_CD_PRODUTO",
                schema: "erp",
                table: "MOVIMENTO_ESTOQUE",
                column: "CD_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDO_COMPRA_CD_FORNECEDOR",
                schema: "erp",
                table: "PEDIDO_COMPRA",
                column: "CD_FORNECEDOR");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDO_VENDA_CD_CLIENTE",
                schema: "erp",
                table: "PEDIDO_VENDA",
                column: "CD_CLIENTE");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_CD_SKU",
                schema: "erp",
                table: "PRODUTO",
                column: "CD_SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PRODUTO_CD_UNIDADE_MEDIDA",
                schema: "erp",
                table: "PRODUTO",
                column: "CD_UNIDADE_MEDIDA");

            migrationBuilder.CreateIndex(
                name: "IX_SALDO_ESTOQUE_CD_PRODUTO_CD_EMPRESA",
                schema: "erp",
                table: "SALDO_ESTOQUE",
                columns: new[] { "CD_PRODUTO", "CD_EMPRESA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UNIDADE_MEDIDA_CD_SIGLA",
                schema: "erp",
                table: "UNIDADE_MEDIDA",
                column: "CD_SIGLA",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ITEM_PEDIDO_COMPRA",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "ITEM_PEDIDO_VENDA",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "MOVIMENTO_ESTOQUE",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "SALDO_ESTOQUE",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PEDIDO_COMPRA",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PEDIDO_VENDA",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "PRODUTO",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "FORNECEDOR",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "CLIENTE",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "UNIDADE_MEDIDA",
                schema: "erp");

            migrationBuilder.DropTable(
                name: "EMPRESA",
                schema: "erp");
        }
    }
}
