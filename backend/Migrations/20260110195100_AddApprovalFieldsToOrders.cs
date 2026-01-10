using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ReservedQuantity",
                table: "Warehouse_Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Transfer_Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Transfer_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Transfer_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Transfer_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transfer_Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Supply_Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Supply_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Supply_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Supply_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Supply_Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Release_Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "Release_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Release_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Release_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Release_Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "Warehouse_Products");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Transfer_Orders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Transfer_Orders");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Transfer_Orders");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Transfer_Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transfer_Orders");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Supply_Orders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Supply_Orders");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Supply_Orders");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Supply_Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Supply_Orders");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Release_Orders");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "Release_Orders");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Release_Orders");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Release_Orders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Release_Orders");
        }
    }
}
