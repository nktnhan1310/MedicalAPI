using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBHYT",
                table: "ServiceTypes");

            migrationBuilder.AddColumn<bool>(
                name: "IsBHYT",
                table: "ServiceTypeMappingHospital",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBHYT",
                table: "ServiceTypeMappingHospital");

            migrationBuilder.AddColumn<bool>(
                name: "IsBHYT",
                table: "ServiceTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
