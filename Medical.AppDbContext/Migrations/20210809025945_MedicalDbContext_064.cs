using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_064 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionServiceId",
                table: "MedicalRecordDetailFiles");

            migrationBuilder.AddColumn<string>(
                name: "DoctorComment",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationScheduleDetailId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorComment",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ExaminationScheduleDetailId",
                table: "MedicalRecordDetails");

            migrationBuilder.AddColumn<int>(
                name: "AdditionServiceId",
                table: "MedicalRecordDetailFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
