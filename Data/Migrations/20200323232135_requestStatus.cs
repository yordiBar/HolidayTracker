using Microsoft.EntityFrameworkCore.Migrations;

namespace HolidayTracker.Data.Migrations
{
    public partial class requestStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestTypeId",
                table: "Requests",
                column: "RequestTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestTypes_RequestTypeId",
                table: "Requests",
                column: "RequestTypeId",
                principalTable: "RequestTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestTypes_RequestTypeId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestTypeId",
                table: "Requests");
        }
    }
}
