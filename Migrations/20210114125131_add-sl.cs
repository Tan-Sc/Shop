using Microsoft.EntityFrameworkCore.Migrations;

namespace doan_webfix.Migrations
{
    public partial class addsl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "amount",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "amount",
               table: "Products");
        }
    }
}
