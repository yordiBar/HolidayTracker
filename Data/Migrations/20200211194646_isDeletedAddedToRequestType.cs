using Microsoft.EntityFrameworkCore.Migrations;

namespace HolidayTracker.Data.Migrations
{
    public partial class isDeletedAddedToRequestType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RequestTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RequestTypes");
        }
    }
}
