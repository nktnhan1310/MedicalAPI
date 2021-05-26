using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationTypeId",
                table: "ExaminationForms");

            migrationBuilder.AddColumn<int>(
                name: "ServiceTypeId",
                table: "ExaminationForms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTypeId",
                table: "ExaminationForms");

            migrationBuilder.AddColumn<int>(
                name: "ExaminationTypeId",
                table: "ExaminationForms",
                type: "int",
                nullable: true);
        }
    }
}
