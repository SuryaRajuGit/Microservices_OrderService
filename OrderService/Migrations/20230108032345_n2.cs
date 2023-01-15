using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Order_Service.Migrations
{
    public partial class n2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Bill");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Bill",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Bill");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
