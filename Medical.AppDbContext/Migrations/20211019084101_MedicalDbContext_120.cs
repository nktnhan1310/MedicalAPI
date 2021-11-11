using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_120 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "UserVaccineProcesses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InjectPlaceDescription",
                table: "UserVaccineProcesses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "UserVaccineProcesses");

            migrationBuilder.DropColumn(
                name: "InjectPlaceDescription",
                table: "UserVaccineProcesses");
        }
    }
}
