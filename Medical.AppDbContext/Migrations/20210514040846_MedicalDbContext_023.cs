using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ExaminationForms",
                newName: "SpecialistTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "ExaminationForms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialistTypeId",
                table: "ExaminationForms",
                newName: "UserId");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "ExaminationForms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
