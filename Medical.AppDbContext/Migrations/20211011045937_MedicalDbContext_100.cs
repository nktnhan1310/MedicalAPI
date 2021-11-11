using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBHYT",
                table: "ServiceTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BarCodeUrl",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagnoticSickName",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagnoticTypeId",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBHYT",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "BarCodeUrl",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "DiagnoticSickName",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "DiagnoticTypeId",
                table: "MedicalRecordDetails");
        }
    }
}
