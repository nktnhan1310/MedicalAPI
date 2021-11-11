using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalDoctor",
                table: "RoomExaminations");

            migrationBuilder.AddColumn<string>(
                name: "ExaminationAreaDescription",
                table: "RoomExaminations",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationAreaDescription",
                table: "RoomExaminations");

            migrationBuilder.AddColumn<int>(
                name: "TotalDoctor",
                table: "RoomExaminations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
