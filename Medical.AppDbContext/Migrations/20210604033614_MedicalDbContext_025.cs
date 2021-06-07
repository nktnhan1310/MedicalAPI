using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_025 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemSendCount",
                table: "SMSConfiguration");

            migrationBuilder.DropColumn(
                name: "MessageTypeId",
                table: "SMSConfiguration");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "SMSConfiguration",
                newName: "WebServiceUrl");

            migrationBuilder.RenameColumn(
                name: "TimeSend",
                table: "SMSConfiguration",
                newName: "SMSType");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "SMSConfiguration",
                newName: "TemplateText");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "SMSConfiguration",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "APIKey",
                table: "SMSConfiguration",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "SMSConfiguration",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIKey",
                table: "SMSConfiguration");

            migrationBuilder.DropColumn(
                name: "SecretKey",
                table: "SMSConfiguration");

            migrationBuilder.RenameColumn(
                name: "WebServiceUrl",
                table: "SMSConfiguration",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "TemplateText",
                table: "SMSConfiguration",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "SMSType",
                table: "SMSConfiguration",
                newName: "TimeSend");

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "SMSConfiguration",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemSendCount",
                table: "SMSConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessageTypeId",
                table: "SMSConfiguration",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
