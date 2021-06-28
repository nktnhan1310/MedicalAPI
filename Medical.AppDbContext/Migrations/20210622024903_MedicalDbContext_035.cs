using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_035 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExaminationDate",
                table: "ExaminationHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExaminationIndex",
                table: "ExaminationHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExaminationPaymentIndex",
                table: "ExaminationHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReExaminationDate",
                table: "ExaminationHistories",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationFormId",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ExaminationDate",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationIndex",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationPaymentIndex",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "ReExaminationDate",
                table: "ExaminationHistories");
        }
    }
}
