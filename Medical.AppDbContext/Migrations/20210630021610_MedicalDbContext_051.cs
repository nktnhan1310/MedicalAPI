using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_051 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "MedicalRecords",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserFullName",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "MedicalRecords",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "MedicalRecords",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
