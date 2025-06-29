using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Migrations
{
    /// <inheritdoc />
    public partial class TransferOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfer_Orders",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<int>(type: "int", nullable: false),
                    To = table.Column<int>(type: "int", nullable: false),
                    T_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfer_Orders", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Transfer_Orders_Suppliers_Supplier_ID",
                        column: x => x.Supplier_ID,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transfer_Orders_Warehouses_From",
                        column: x => x.From,
                        principalTable: "Warehouses",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfer_Orders_Warehouses_To",
                        column: x => x.To,
                        principalTable: "Warehouses",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TO_Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TO_Amount = table.Column<double>(type: "float", nullable: false),
                    TO_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TO_Price = table.Column<double>(type: "float", nullable: false),
                    TO_MFD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TO_EXP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TO_Number = table.Column<int>(type: "int", nullable: false),
                    Product_Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TO_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TO_Products_Products_Product_Code",
                        column: x => x.Product_Code,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TO_Products_Transfer_Orders_TO_Number",
                        column: x => x.TO_Number,
                        principalTable: "Transfer_Orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TO_Products_Product_Code",
                table: "TO_Products",
                column: "Product_Code");

            migrationBuilder.CreateIndex(
                name: "IX_TO_Products_TO_Number",
                table: "TO_Products",
                column: "TO_Number");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_Orders_From",
                table: "Transfer_Orders",
                column: "From");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_Orders_Supplier_ID",
                table: "Transfer_Orders",
                column: "Supplier_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Transfer_Orders_To",
                table: "Transfer_Orders",
                column: "To");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TO_Products");

            migrationBuilder.DropTable(
                name: "Transfer_Orders");
        }
    }
}
