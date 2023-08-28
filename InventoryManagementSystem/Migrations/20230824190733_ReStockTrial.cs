using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ReStockTrial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReStockItem",
                columns: table => new
                {
                    ReStockID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemConsumableId = table.Column<int>(type: "INTEGER", nullable: false),
                    KodeItemConsumable = table.Column<string>(type: "TEXT", nullable: true),
                    RequestStockDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReStockItem", x => x.ReStockID);
                    table.ForeignKey(
                        name: "FK_ReStockItem_ItemsConsumable_ItemConsumableId",
                        column: x => x.ItemConsumableId,
                        principalTable: "ItemsConsumable",
                        principalColumn: "IdItemConsumable",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReStockItem_ItemConsumableId",
                table: "ReStockItem",
                column: "ItemConsumableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReStockItem");
        }
    }
}
