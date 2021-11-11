using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_080 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "UserFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalRecordDetailId",
                table: "UserFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalRecordId",
                table: "UserFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "MedicalRecordFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "MedicalRecordDetailFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "MedicalRecordDetailFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFolders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFolders");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "MedicalRecordDetailId",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "MedicalRecordId",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "MedicalRecordFiles");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "MedicalRecordDetailFiles");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "MedicalRecordDetailFiles");
        }
    }
}
