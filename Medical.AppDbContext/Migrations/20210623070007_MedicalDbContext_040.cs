using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_040 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "MedicalRecordDetailFiles",
                newName: "AdditionServiceId");

            migrationBuilder.AddColumn<int>(
                name: "AdditionServiceId",
                table: "PaymentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormDetailId",
                table: "PaymentHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationFormDetailId",
                table: "MedicalRecordDetailFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdditionServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    HospitalId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExaminationFormDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExaminationFormId = table.Column<int>(type: "int", nullable: true),
                    AdditionServiceId = table.Column<int>(type: "int", nullable: false),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: true),
                    AdditionExaminationIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ExaminationFormDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionServices");

            migrationBuilder.DropTable(
                name: "ExaminationFormDetails");

            migrationBuilder.DropColumn(
                name: "AdditionServiceId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationFormDetailId",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "ExaminationFormDetailId",
                table: "MedicalRecordDetailFiles");

            migrationBuilder.RenameColumn(
                name: "AdditionServiceId",
                table: "MedicalRecordDetailFiles",
                newName: "FileType");
        }
    }
}
