using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_128 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImportScheduleId",
                table: "ExaminationSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImportScheduleId",
                table: "ExaminationScheduleDetails",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImportScheduleId",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "ImportScheduleId",
                table: "ExaminationScheduleDetails");
        }
    }
}
