using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_116 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigTimeExaminationId",
                table: "ExaminationScheduleDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfigTimeExaminationId",
                table: "ExaminationScheduleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
