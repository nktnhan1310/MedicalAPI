using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_118 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForChild",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "IsForPregnant",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "VaccineTypeId",
                table: "VaccineTypes");

            migrationBuilder.AddColumn<string>(
                name: "TargetIdValues",
                table: "VaccineTypes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetIdValues",
                table: "VaccineTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsForChild",
                table: "VaccineTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsForPregnant",
                table: "VaccineTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "VaccineTypeId",
                table: "VaccineTypes",
                type: "int",
                nullable: true);
        }
    }
}
