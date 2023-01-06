using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Order_Service.Migrations
{
    public partial class new3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BillNo = table.Column<int>(nullable: false),
                    cartId = table.Column<Guid>(nullable: true),
                    OrderValue = table.Column<float>(nullable: false),
                    PaymentType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_Cart_cartId",
                        column: x => x.cartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_cartId",
                table: "Bill",
                column: "cartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillNo = table.Column<int>(type: "int", nullable: false),
                    OrderValue = table.Column<float>(type: "real", nullable: false),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Cart_cartId",
                        column: x => x.cartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_cartId",
                table: "Payment",
                column: "cartId");
        }
    }
}
