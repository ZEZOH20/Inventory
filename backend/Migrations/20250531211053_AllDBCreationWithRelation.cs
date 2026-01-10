using System;
using Microsoft.EntityFrameworkCore.Migrations;
using backend.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AllDBCreationWithRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Release_Orders",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_ID = table.Column<int>(type: "int", nullable: false),
                    War_Number = table.Column<int>(type: "int", nullable: false),
                    R_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Release_Orders", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Release_Orders_Customers_Customer_ID",
                        column: x => x.Customer_ID,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Release_Orders_Warehouses_War_Number",
                        column: x => x.War_Number,
                        principalTable: "Warehouses",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supply_Orders",
                columns: table => new
                {
                    Number = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Supplier_ID = table.Column<int>(type: "int", nullable: false),
                    War_Number = table.Column<int>(type: "int", nullable: false),
                    S_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supply_Orders", x => x.Number);
                    table.ForeignKey(
                        name: "FK_Supply_Orders_Suppliers_Supplier_ID",
                        column: x => x.Supplier_ID,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supply_Orders_Warehouses_War_Number",
                        column: x => x.War_Number,
                        principalTable: "Warehouses",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RO_Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RO_Amount = table.Column<double>(type: "float", nullable: false),
                    RO_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RO_Price = table.Column<double>(type: "float", nullable: false),
                    RO_Number = table.Column<int>(type: "int", nullable: false),
                    Product_Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RO_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RO_Product_Products_Product_Code",
                        column: x => x.Product_Code,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RO_Product_Release_Orders_RO_Number",
                        column: x => x.RO_Number,
                        principalTable: "Release_Orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SO_Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SO_Amount = table.Column<double>(type: "float", nullable: false),
                    SO_Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SO_Price = table.Column<double>(type: "float", nullable: false),
                    SO_MFD = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SO_EXP = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SO_Number = table.Column<int>(type: "int", nullable: false),
                    Product_Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SO_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SO_Products_Products_Product_Code",
                        column: x => x.Product_Code,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SO_Products_Supply_Orders_SO_Number",
                        column: x => x.SO_Number,
                        principalTable: "Supply_Orders",
                        principalColumn: "Number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Release_Orders_Customer_ID",
                table: "Release_Orders",
                column: "Customer_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Release_Orders_War_Number",
                table: "Release_Orders",
                column: "War_Number");

            migrationBuilder.CreateIndex(
                name: "IX_RO_Product_Product_Code",
                table: "RO_Product",
                column: "Product_Code");

            migrationBuilder.CreateIndex(
                name: "IX_RO_Product_RO_Number",
                table: "RO_Product",
                column: "RO_Number");

            migrationBuilder.CreateIndex(
                name: "IX_SO_Products_Product_Code",
                table: "SO_Products",
                column: "Product_Code");

            migrationBuilder.CreateIndex(
                name: "IX_SO_Products_SO_Number",
                table: "SO_Products",
                column: "SO_Number");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_Orders_Supplier_ID",
                table: "Supply_Orders",
                column: "Supplier_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supply_Orders_War_Number",
                table: "Supply_Orders",
                column: "War_Number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RO_Product");

            migrationBuilder.DropTable(
                name: "SO_Products");

            migrationBuilder.DropTable(
                name: "Release_Orders");

            migrationBuilder.DropTable(
                name: "Supply_Orders");
        }
    }
}
