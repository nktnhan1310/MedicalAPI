using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_079 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                table: "MedicalRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Height",
                table: "MedicalRecords",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "MedicalRecords",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodSugar",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeartBeat",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodPressure",
                table: "ExaminationForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodSugar",
                table: "ExaminationForms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeartBeat",
                table: "ExaminationForms",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloodType",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "BloodSugar",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "HeartBeat",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "BloodPressure",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "BloodSugar",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "HeartBeat",
                table: "ExaminationForms");
        }
    }
}
