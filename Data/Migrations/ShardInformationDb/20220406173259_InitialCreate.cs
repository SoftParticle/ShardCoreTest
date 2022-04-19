using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShardCoreTest.Data.Migrations.ShardInformationDb
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShardInformations",
                columns: table => new
                {
                    ShardInformationId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShardFriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShardIdPrefix = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    WriteEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UpdateEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ReadEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatabaseServerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatabaseSchema = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ShardInformationId", x => x.ShardInformationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShardInformations_Enabled",
                table: "ShardInformations",
                column: "Enabled")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ShardInformations_ReadEnabled",
                table: "ShardInformations",
                column: "ReadEnabled")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ShardInformations_ShardIdPrefix",
                table: "ShardInformations",
                column: "ShardIdPrefix",
                unique: true,
                filter: "[ShardIdPrefix] IS NOT NULL")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ShardInformations_UpdateDate",
                table: "ShardInformations",
                column: "UpdateDate")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ShardInformations_WriteEnabled",
                table: "ShardInformations",
                column: "WriteEnabled")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShardInformations");
        }
    }
}
