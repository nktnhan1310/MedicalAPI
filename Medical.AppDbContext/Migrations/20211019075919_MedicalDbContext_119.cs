using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_119 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VaccineId",
                table: "UserVaccineProcesses",
                newName: "VaccineTypeId");

            migrationBuilder.AddColumn<int>(
                name: "MedicalRecordId",
                table: "UserVaccineProcesses",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalRecordId",
                table: "UserVaccineProcesses");

            migrationBuilder.RenameColumn(
                name: "VaccineTypeId",
                table: "UserVaccineProcesses",
                newName: "VaccineId");
        }
    }
}
