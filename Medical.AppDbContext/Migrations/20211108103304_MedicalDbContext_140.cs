using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_140 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationFormId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ExaminationFormIds",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationFormIds",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }
    }
}
