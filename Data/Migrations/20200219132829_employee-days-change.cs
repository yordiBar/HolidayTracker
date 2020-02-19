using Microsoft.EntityFrameworkCore.Migrations;

namespace HolidayTracker.Data.Migrations
{
    public partial class employeedayschange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeWorkingDays");

            migrationBuilder.AddColumn<bool>(
                name: "Fri",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprover",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManager",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Mon",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sat",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sun",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Thu",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Tue",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Wed",
                table: "Employees",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fri",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsApprover",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsManager",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Mon",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Sat",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Sun",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Thu",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Tue",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Wed",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "EmployeeWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Fri = table.Column<bool>(type: "bit", nullable: false),
                    Mon = table.Column<bool>(type: "bit", nullable: false),
                    Sat = table.Column<bool>(type: "bit", nullable: false),
                    Sun = table.Column<bool>(type: "bit", nullable: false),
                    Thu = table.Column<bool>(type: "bit", nullable: false),
                    Tue = table.Column<bool>(type: "bit", nullable: false),
                    Wed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeWorkingDays", x => x.Id);
                });
        }
    }
}
