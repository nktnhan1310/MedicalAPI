using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_130 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReExamination",
                table: "MedicalRecordDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReExaminationDoctorId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReExaminationScheduleDetailId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReRoomExaminationId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomExaminationId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReExamination",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ReExaminationDoctorId",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ReExaminationScheduleDetailId",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ReRoomExaminationId",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "RoomExaminationId",
                table: "MedicalRecordDetails");
        }
    }
}
