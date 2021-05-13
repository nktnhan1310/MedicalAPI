using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_013 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistricId",
                table: "Wards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Nations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "Districts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Cities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistricId",
                table: "Wards");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Nations");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Cities");
        }
    }
}
