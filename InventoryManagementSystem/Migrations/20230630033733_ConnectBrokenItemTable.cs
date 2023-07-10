using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class ConnectBrokenItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokenItems_BorrowedItems_BorrowedItemBorrowedId",
                table: "BrokenItems");

            migrationBuilder.DropIndex(
                name: "IX_BrokenItems_BorrowedItemBorrowedId",
                table: "BrokenItems");

            migrationBuilder.DropColumn(
                name: "BorrowedItemBorrowedId",
                table: "BrokenItems");

            migrationBuilder.AddColumn<int>(
                name: "BrokenId",
                table: "BorrowedItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokenItems_BorrowedId",
                table: "BrokenItems",
                column: "BorrowedId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BrokenItems_BorrowedItems_BorrowedId",
                table: "BrokenItems",
                column: "BorrowedId",
                principalTable: "BorrowedItems",
                principalColumn: "BorrowedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokenItems_BorrowedItems_BorrowedId",
                table: "BrokenItems");

            migrationBuilder.DropIndex(
                name: "IX_BrokenItems_BorrowedId",
                table: "BrokenItems");

            migrationBuilder.DropColumn(
                name: "BrokenId",
                table: "BorrowedItems");

            migrationBuilder.AddColumn<int>(
                name: "BorrowedItemBorrowedId",
                table: "BrokenItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrokenItems_BorrowedItemBorrowedId",
                table: "BrokenItems",
                column: "BorrowedItemBorrowedId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrokenItems_BorrowedItems_BorrowedItemBorrowedId",
                table: "BrokenItems",
                column: "BorrowedItemBorrowedId",
                principalTable: "BorrowedItems",
                principalColumn: "BorrowedId");
        }
    }
}
