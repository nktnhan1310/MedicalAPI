using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "UserAllergies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "UserAllergies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiagnoticTypeId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "UserAllergies");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "UserAllergies");

            migrationBuilder.AlterColumn<string>(
                name: "DiagnoticTypeId",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
