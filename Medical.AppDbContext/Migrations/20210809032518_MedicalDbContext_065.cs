using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_065 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExaminationIndex",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExaminationPaymentIndex",
                table: "MedicalRecordDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "MedicalRecordDetails",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationIndex",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "ExaminationPaymentIndex",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "MedicalRecordDetails");
        }
    }
}
