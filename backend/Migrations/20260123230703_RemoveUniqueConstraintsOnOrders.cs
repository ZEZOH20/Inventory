using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintsOnOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop unique index on Supply_Orders.Supplier_ID and recreate as non-unique
            migrationBuilder.DropIndex(
                name: "IX_Supply_Orders_Supplier_ID",
                table: "Supply_Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_Orders_Supplier_ID",
                table: "Supply_Orders",
                column: "Supplier_ID");

            // Drop unique index on Release_Orders.Customer_ID and recreate as non-unique
            migrationBuilder.DropIndex(
                name: "IX_Release_Orders_Customer_ID",
                table: "Release_Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Release_Orders_Customer_ID",
                table: "Release_Orders",
                column: "Customer_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate unique indexes (reverse of Up)
            migrationBuilder.DropIndex(
                name: "IX_Supply_Orders_Supplier_ID",
                table: "Supply_Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_Orders_Supplier_ID",
                table: "Supply_Orders",
                column: "Supplier_ID",
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Release_Orders_Customer_ID",
                table: "Release_Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Release_Orders_Customer_ID",
                table: "Release_Orders",
                column: "Customer_ID",
                unique: true);
        }
    }
}
