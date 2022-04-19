using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShardCoreTest.Data.Migrations.ShardDb
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ShardId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    GloballyUniqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ShardId", x => x.ShardId);
                    table.UniqueConstraint("AK_Products_GloballyUniqueId", x => x.GloballyUniqueId);
                });

            migrationBuilder.CreateTable(
                name: "ProductProperties",
                columns: table => new
                {
                    ProductPropertyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ProductPropertyId", x => x.ProductPropertyId);
                    table.ForeignKey(
                        name: "FK_ProductProperties_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "GloballyUniqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperties_ProductId",
                table: "ProductProperties",
                column: "ProductId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Description",
                table: "Products",
                column: "Description")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_GloballyUniqueId",
                table: "Products",
                column: "GloballyUniqueId",
                unique: true)
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Model",
                table: "Products",
                column: "Model")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Model_Desc] ON[Products]( [Model] DESC)",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Description_Desc] ON[Products]( [Description] DESC)",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Price_Desc] ON[Products]( [Price] DESC)",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Products(Model) KEY INDEX ShardId;",
                suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductProperties");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
