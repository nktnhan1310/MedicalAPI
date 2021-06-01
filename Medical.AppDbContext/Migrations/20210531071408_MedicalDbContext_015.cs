using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_015 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Users",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Users");
        }
    }
}
