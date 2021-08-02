using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExaminationScheduleDetailId",
                table: "ExaminationHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomExaminationId",
                table: "ExaminationHistories",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationScheduleDetailId",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "RoomExaminationId",
                table: "ExaminationHistories");
        }
    }
}
