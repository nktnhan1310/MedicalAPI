using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUseHospitalConfig",
                table: "ExaminationSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUseHospitalConfig",
                table: "ExaminationScheduleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionTypeId",
                table: "ExaminationScheduleDetails",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUseHospitalConfig",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "IsUseHospitalConfig",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "SessionTypeId",
                table: "ExaminationScheduleDetails");
        }
    }
}
