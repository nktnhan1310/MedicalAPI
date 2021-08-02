using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_054 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormDetailId",
                table: "MomoPayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalBillId",
                table: "MomoPayments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SystemConfigFee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fee = table.Column<double>(type: "float", nullable: true),
                    IsCheckRate = table.Column<bool>(type: "bit", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemConfigFee", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemConfigFee");

            migrationBuilder.DropColumn(
                name: "ExaminationFormDetailId",
                table: "MomoPayments");

            migrationBuilder.DropColumn(
                name: "MedicalBillId",
                table: "MomoPayments");
        }
    }
}
