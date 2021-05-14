using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_021 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "SessionTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalDoctor",
                table: "RoomExaminations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExaminationIndex",
                table: "ExaminationForms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "ConfigTimeExaminations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConfigRoomExaminations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomExaminationId = table.Column<int>(type: "int", nullable: false),
                    TotalPatient = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigRoomExaminations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigRoomExaminations");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "TotalDoctor",
                table: "RoomExaminations");

            migrationBuilder.DropColumn(
                name: "ExaminationIndex",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "ConfigTimeExaminations");
        }
    }
}
