using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_089 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaximumAfternoonExamination",
                table: "ExaminationSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumMorningExamination",
                table: "ExaminationSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumOtherExamination",
                table: "ExaminationSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaximumExamination",
                table: "ExaminationScheduleDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BHYTType",
                table: "ExaminationForms",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumAfternoonExamination",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "MaximumMorningExamination",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "MaximumOtherExamination",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "MaximumExamination",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "BHYTType",
                table: "ExaminationForms");
        }
    }
}
