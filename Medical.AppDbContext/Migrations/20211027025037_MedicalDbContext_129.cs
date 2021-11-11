using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_129 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromTimeExamination",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FromTimeExaminationText",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToTimeExamination",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ToTimeExaminationText",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromTimeExamination",
                table: "ExaminationForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FromTimeExaminationText",
                table: "ExaminationForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToTimeExamination",
                table: "ExaminationForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ToTimeExaminationText",
                table: "ExaminationForms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromTimeExamination",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "FromTimeExaminationText",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ToTimeExamination",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ToTimeExaminationText",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "FromTimeExamination",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "FromTimeExaminationText",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "ToTimeExamination",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "ToTimeExaminationText",
                table: "ExaminationForms");
        }
    }
}
