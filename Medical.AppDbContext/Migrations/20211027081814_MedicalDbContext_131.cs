using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_131 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReFromTimeExaminationText",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReToTimeExaminationText",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReFromTimeExaminationText",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ReToTimeExaminationText",
                table: "MedicalRecordDetails");
        }
    }
}
