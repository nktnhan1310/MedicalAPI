using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_155 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateTypeId",
                table: "VaccineTypes");

            migrationBuilder.DropColumn(
                name: "NumberOfDateTypeValue",
                table: "VaccineTypes");

            migrationBuilder.CreateTable(
                name: "VaccineTypeDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaccineTypeId = table.Column<int>(type: "int", nullable: true),
                    MonthValue = table.Column<int>(type: "int", nullable: true),
                    IsRepeat = table.Column<bool>(type: "bit", nullable: false),
                    MonthRepeatValue = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    HospitalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccineTypeDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaccineTypeDetails");

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
        }
    }
}
