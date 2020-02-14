using Microsoft.EntityFrameworkCore.Migrations;

namespace HolidayTracker.Data.Migrations
{
    public partial class Allowance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TakesFromAllowance",
                table: "RequestTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TakesFromAllowance",
                table: "RequestTypes");
        }
    }
}
