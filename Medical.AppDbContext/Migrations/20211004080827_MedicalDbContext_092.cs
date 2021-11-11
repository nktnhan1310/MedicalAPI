using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_092 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackGroundImgUrl",
                table: "NewFeeds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplaceDoctorId",
                table: "ExaminationSchedules",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackGroundImgUrl",
                table: "NewFeeds");

            migrationBuilder.DropColumn(
                name: "ReplaceDoctorId",
                table: "ExaminationSchedules");
        }
    }
}
