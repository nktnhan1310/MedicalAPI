using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_024 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "NotificationTypes");

            migrationBuilder.AddColumn<int>(
                name: "UserGroupId",
                table: "NotificationApplicationUser",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserGroupId",
                table: "NotificationApplicationUser");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "NotificationTypes",
                type: "int",
                nullable: true);
        }
    }
}
