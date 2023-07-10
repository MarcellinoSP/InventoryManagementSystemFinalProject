using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class PembuatanBrokenItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "KodeItem",
                table: "Items",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "BrokenItems",
                columns: table => new
                {
                    BrokenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BrokenDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteItemBroken = table.Column<string>(type: "TEXT", nullable: true),
                    NoteItemFound = table.Column<string>(type: "TEXT", nullable: true),
                    BorrowedId = table.Column<int>(type: "INTEGER", nullable: true),
                    BorrowedItemBorrowedId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokenItems", x => x.BrokenId);
                    table.ForeignKey(
                        name: "FK_BrokenItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrokenItems_BorrowedItems_BorrowedItemBorrowedId",
                        column: x => x.BorrowedItemBorrowedId,
                        principalTable: "BorrowedItems",
                        principalColumn: "BorrowedId");
                    table.ForeignKey(
                        name: "FK_BrokenItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokenItems_BorrowedItemBorrowedId",
                table: "BrokenItems",
                column: "BorrowedItemBorrowedId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokenItems_ItemId",
                table: "BrokenItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokenItems_UserId",
                table: "BrokenItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokenItems");

            migrationBuilder.AlterColumn<string>(
                name: "KodeItem",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
