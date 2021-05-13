using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "ExaminationSchedules");

            migrationBuilder.AddColumn<int>(
                name: "SpecialistTypeId",
                table: "SpecialistTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ConfigTimeExaminations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecialistTypeId",
                table: "SpecialistTypes");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ConfigTimeExaminations");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "ExaminationSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
