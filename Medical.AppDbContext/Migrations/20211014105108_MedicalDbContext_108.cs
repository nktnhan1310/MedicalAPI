using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_108 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DateTypeId",
                table: "VaccineTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDateTypeValue",
                table: "VaccineTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "NewFeeds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "NewFeeds",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTypeId",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "NumberOfDateTypeValue",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "NewFeeds");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "NewFeeds");
        }
    }
}
