using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Migrations
{
    /// <inheritdoc />
    public partial class Warehouse_ProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Code = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse_Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Code = table.Column<int>(type: "int", nullable: false),
                    War_Number = table.Column<int>(type: "int", nullable: false),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    MFD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EXP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Store_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total_Amount = table.Column<double>(type: "float", nullable: false),
                    Total_Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_Products_Products_Product_Code",
                        column: x => x.Product_Code,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warehouse_Products_Suppliers_Supplier_ID",
                        column: x => x.Supplier_ID,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warehouse_Products_Warehouses_War_Number",
                        column: x => x.War_Number,
                        principalTable: "Warehouses",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Products_Product_Code",
                table: "Warehouse_Products",
                column: "Product_Code");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Products_Supplier_ID",
                table: "Warehouse_Products",
                column: "Supplier_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Products_War_Number",
                table: "Warehouse_Products",
                column: "War_Number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Warehouse_Products");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
