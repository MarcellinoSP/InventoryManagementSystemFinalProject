using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddModelAllConsumableItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemsConsumable",
                columns: table => new
                {
                    IdItemConsumable = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KodeItemConsumable = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PicturePath = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Availability = table.Column<bool>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsConsumable", x => x.IdItemConsumable);
                    table.ForeignKey(
                        name: "FK_ItemsConsumable_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "IdCategory",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsConsumable_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "IdSubCategory",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsConsumable_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestItemsConsumable",
                columns: table => new
                {
                    RequestConsumableId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderItemConsumableId = table.Column<int>(type: "INTEGER", nullable: true),
                    ItemConsumableId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RequestConsumeDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    NoteRequest = table.Column<string>(type: "TEXT", nullable: false),
                    NoteActionRequest = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestItemsConsumable", x => x.RequestConsumableId);
                    table.ForeignKey(
                        name: "FK_RequestItemsConsumable_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestItemsConsumable_ItemsConsumable_ItemConsumableId",
                        column: x => x.ItemConsumableId,
                        principalTable: "ItemsConsumable",
                        principalColumn: "IdItemConsumable",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemsConsumable",
                columns: table => new
                {
                    OrderConsumableId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    ConsumableId = table.Column<int>(type: "INTEGER", nullable: true),
                    ItemConsumableId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConsumeDateApproved = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteDonePickUp = table.Column<string>(type: "TEXT", nullable: false),
                    NoteWaitingPickUp = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemsConsumable", x => x.OrderConsumableId);
                    table.ForeignKey(
                        name: "FK_OrderItemsConsumable_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemsConsumable_ItemsConsumable_ItemConsumableId",
                        column: x => x.ItemConsumableId,
                        principalTable: "ItemsConsumable",
                        principalColumn: "IdItemConsumable",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemsConsumable_RequestItemsConsumable_RequestId",
                        column: x => x.RequestId,
                        principalTable: "RequestItemsConsumable",
                        principalColumn: "RequestConsumableId");
                });

            migrationBuilder.CreateTable(
                name: "ConsumedItems",
                columns: table => new
                {
                    ConsumedId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    ItemConsumableId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConsumedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NoteConsumed = table.Column<string>(type: "TEXT", nullable: false),
                    PicturePath = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumedItems", x => x.ConsumedId);
                    table.ForeignKey(
                        name: "FK_ConsumedItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumedItems_ItemsConsumable_ItemConsumableId",
                        column: x => x.ItemConsumableId,
                        principalTable: "ItemsConsumable",
                        principalColumn: "IdItemConsumable",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumedItems_OrderItemsConsumable_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderItemsConsumable",
                        principalColumn: "OrderConsumableId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumedItems_ItemConsumableId",
                table: "ConsumedItems",
                column: "ItemConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumedItems_OrderId",
                table: "ConsumedItems",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumedItems_UserId",
                table: "ConsumedItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsConsumable_CategoryId",
                table: "ItemsConsumable",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsConsumable_SubCategoryId",
                table: "ItemsConsumable",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsConsumable_SupplierId",
                table: "ItemsConsumable",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsConsumable_ItemConsumableId",
                table: "OrderItemsConsumable",
                column: "ItemConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsConsumable_RequestId",
                table: "OrderItemsConsumable",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemsConsumable_UserId",
                table: "OrderItemsConsumable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestItemsConsumable_ItemConsumableId",
                table: "RequestItemsConsumable",
                column: "ItemConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestItemsConsumable_UserId",
                table: "RequestItemsConsumable",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumedItems");

            migrationBuilder.DropTable(
                name: "OrderItemsConsumable");

            migrationBuilder.DropTable(
                name: "RequestItemsConsumable");

            migrationBuilder.DropTable(
                name: "ItemsConsumable");
        }
    }
}
