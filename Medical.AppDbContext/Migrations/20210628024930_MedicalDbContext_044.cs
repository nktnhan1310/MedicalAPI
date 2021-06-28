using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_044 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "PaymentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalBillId",
                table: "PaymentHistories",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "MedicalBillId",
                table: "PaymentHistories");
        }
    }
}
