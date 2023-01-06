using Microsoft.EntityFrameworkCore.Migrations;

namespace Order_Service.Migrations
{
    public partial class n5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Bill",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Bill");
        }
    }
}
