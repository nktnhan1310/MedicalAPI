using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelMappingHospital_Hospitals_HospitalsId",
                table: "ChannelMappingHospital");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTypeMappingHospital_Hospitals_HospitalsId",
                table: "ServiceTypeMappingHospital");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTypeMappingHospital_HospitalsId",
                table: "ServiceTypeMappingHospital");

            migrationBuilder.DropIndex(
                name: "IX_ChannelMappingHospital_HospitalsId",
                table: "ChannelMappingHospital");

            migrationBuilder.DropColumn(
                name: "HospitalsId",
                table: "ServiceTypeMappingHospital");

            migrationBuilder.DropColumn(
                name: "HospitalsId",
                table: "ChannelMappingHospital");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalsId",
                table: "ServiceTypeMappingHospital",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalsId",
                table: "ChannelMappingHospital",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypeMappingHospital_HospitalsId",
                table: "ServiceTypeMappingHospital",
                column: "HospitalsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelMappingHospital_HospitalsId",
                table: "ChannelMappingHospital",
                column: "HospitalsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelMappingHospital_Hospitals_HospitalsId",
                table: "ChannelMappingHospital",
                column: "HospitalsId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTypeMappingHospital_Hospitals_HospitalsId",
                table: "ServiceTypeMappingHospital",
                column: "HospitalsId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
