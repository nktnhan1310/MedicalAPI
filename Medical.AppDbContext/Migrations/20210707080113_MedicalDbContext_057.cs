using Microsoft.EntityFrameworkCore.Migrations;

namespace Medical.AppDbContext.Migrations
{
    public partial class MedicalDbContext_057 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Wards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "UserInGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "UserGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "UserFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SystemConfiguartions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SystemConfigFee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SpecialistTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SMSEmailTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SMSConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "SessionTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ServiceTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ServiceTypeMappingHospital",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "RoomExaminations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Relations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "PermitObjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "PermitObjectPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "PaymentMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "PaymentHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "OTPHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "NotificationTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "NotificationApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Nations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MomoPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MomoConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Medicines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalRecordFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalRecordDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalRecordDetailFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalRecordAdditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "MedicalBills",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "HospitalFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "HospitalConfigFee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationScheduleDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationForms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ExaminationFormDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "EmailConfiguration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "DoctorDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Districts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "DegreeTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Countries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ContentReplaceCharacters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ConfigTimeExaminations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ConfigRoomExaminations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Cities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "ChannelMappingHospital",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "BankInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "AppointmentSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckOTP",
                table: "AdditionServices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Wards");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "UserInGroups");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "UserGroups");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SystemConfiguartions");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SystemConfigFee");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SpecialistTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SMSEmailTemplates");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SMSConfiguration");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ServiceTypeMappingHospital");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "RoomExaminations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Relations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "PermitObjects");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "PermitObjectPermissions");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "OTPHistories");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "NotificationTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "NotificationApplicationUser");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Nations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MomoPayments");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MomoConfigurations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalRecordFiles");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalRecordDetails");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalRecordDetailFiles");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalRecordAdditions");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "MedicalBills");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "HospitalFiles");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "HospitalConfigFee");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationSchedules");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationScheduleDetails");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationHistories");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationForms");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ExaminationFormDetails");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "EmailConfiguration");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "DoctorDetails");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "DegreeTypes");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ContentReplaceCharacters");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ConfigTimeExaminations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ConfigRoomExaminations");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "ChannelMappingHospital");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "BankInfos");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "AppointmentSchedules");

            migrationBuilder.DropColumn(
                name: "IsCheckOTP",
                table: "AdditionServices");
        }
    }
}
