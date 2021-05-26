using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BankInfoId",
                table: "PaymentHistories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BankInfo",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "PaymentHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodName",
                table: "PaymentHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "ExaminationForms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankInfo",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "PaymentMethodName",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "ExaminationForms");

            migrationBuilder.AlterColumn<int>(
                name: "BankInfoId",
                table: "PaymentHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
