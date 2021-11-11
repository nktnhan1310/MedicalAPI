using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_109 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SessionTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "FromTime",
                table: "SessionTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "SessionTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToTime",
                table: "SessionTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromTime",
                table: "ExaminationScheduleDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromTimeText",
                table: "ExaminationScheduleDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToTime",
                table: "ExaminationScheduleDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToTimeText",
                table: "ExaminationScheduleDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "ToTime",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "FromTime",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "FromTimeText",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "ToTime",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "ToTimeText",
                table: "ExaminationScheduleDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SessionTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
