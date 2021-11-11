using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_087 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalCancelExaminations",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalEditExaminations",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdit",
                table: "ExaminationHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ExaminationHistories",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCancelExaminations",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalEditExaminations",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEdit",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ExaminationHistories");
        }
    }
}
