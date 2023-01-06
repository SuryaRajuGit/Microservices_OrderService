using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Order_Service.Migrations
{
    public partial class new1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Cart_BillNoId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_BillNoId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "BillNoId",
                table: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "BillNo",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "cartId",
                table: "Payment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillNo",
                table: "Cart",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_cartId",
                table: "Payment",
                column: "cartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Cart_cartId",
                table: "Payment",
                column: "cartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Cart_cartId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_cartId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "BillNo",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "cartId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "BillNo",
                table: "Cart");

            migrationBuilder.AddColumn<int>(
                name: "BillId",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "BillNoId",
                table: "Payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BillNoId",
                table: "Payment",
                column: "BillNoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Cart_BillNoId",
                table: "Payment",
                column: "BillNoId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
