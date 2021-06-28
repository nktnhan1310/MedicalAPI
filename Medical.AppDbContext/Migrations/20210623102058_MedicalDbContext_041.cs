using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_041 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankInfo",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "PaymentMethodName",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "Prescription",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "PrescriptionCode",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ExaminationFormDetailId",
                table: "MedicalRecordDetailFiles");

            migrationBuilder.AlterColumn<int>(
                name: "ExaminationFormId",
                table: "PaymentHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExaminationDate",
                table: "ExaminationFormDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationDate",
                table: "ExaminationFormDetails");

            migrationBuilder.AlterColumn<int>(
                name: "ExaminationFormId",
                table: "PaymentHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankInfo",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodName",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prescription",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrescriptionCode",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormDetailId",
                table: "MedicalRecordDetailFiles",
                type: "int",
                nullable: true);
        }
    }
}
