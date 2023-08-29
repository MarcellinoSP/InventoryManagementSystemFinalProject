using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class EditDateTimeInReStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReStockItem_Suppliers_SupplierId",
                table: "ReStockItem");

            // migrationBuilder.DropIndex(
            //     name: "IX_ReStockItem_SupplierId",
            //     table: "ReStockItem");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ReStockItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "ReStockItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReStockItem_SupplierId",
                table: "ReStockItem",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReStockItem_Suppliers_SupplierId",
                table: "ReStockItem",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
