using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_132 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalRecordHistoryId",
                table: "UserFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SurgeryDate",
                table: "MedicalRecordHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurgeryIndication",
                table: "MedicalRecordHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicalRecordHistoryId",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "SurgeryDate",
                table: "MedicalRecordHistories");

            migrationBuilder.DropColumn(
                name: "SurgeryIndication",
                table: "MedicalRecordHistories");
        }
    }
}
