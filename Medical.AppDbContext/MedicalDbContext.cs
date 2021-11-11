using Medical.Entities;
using Medical.Interface.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Text;

namespace Medical.AppDbContext
{
    public class MedicalDbContext : DbContext, IMedicalDbContext
    {
        public MedicalDbContext(DbContextOptions<MedicalDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Catalogue

            modelBuilder.Entity<Channels>(x => x.ToTable("Channels"));
            modelBuilder.Entity<Cities>(x => x.ToTable("Cities"));
            modelBuilder.Entity<ConfigTimeExaminations>(x => x.ToTable("ConfigTimeExaminations"));
            modelBuilder.Entity<Countries>(x => x.ToTable("Countries"));
            modelBuilder.Entity<DegreeTypes>(x => x.ToTable("DegreeTypes"));
            modelBuilder.Entity<Districts>(x => x.ToTable("Districts"));
            modelBuilder.Entity<ExaminationTypes>(x => x.ToTable("ExaminationTypes"));
            modelBuilder.Entity<Jobs>(x => x.ToTable("Jobs"));
            modelBuilder.Entity<Nations>(x => x.ToTable("Nations"));
            modelBuilder.Entity<Notifications>(x => x.ToTable("Notifications"));
            modelBuilder.Entity<NotificationTypes>(x => x.ToTable("NotificationTypes"));
            modelBuilder.Entity<RoomExaminations>(x => x.ToTable("RoomExaminations"));
            modelBuilder.Entity<ServiceTypes>(x => x.ToTable("ServiceTypes"));
            modelBuilder.Entity<SessionTypes>(x => x.ToTable("SessionTypes"));
            modelBuilder.Entity<SpecialistTypes>(x => x.ToTable("SpecialistTypes"));
            modelBuilder.Entity<Wards>(x => x.ToTable("Wards"));
            modelBuilder.Entity<Relations>(x => x.ToTable("Relations"));
            modelBuilder.Entity<PaymentMethods>(x => x.ToTable("PaymentMethods"));
            modelBuilder.Entity<AdditionServices>(x => x.ToTable("AdditionServices"));
            modelBuilder.Entity<AdditionServiceDetails>(x => x.ToTable("AdditionServiceDetails"));
            modelBuilder.Entity<MedicalBills>(x => x.ToTable("MedicalBills"));
            modelBuilder.Entity<Medicines>(x => x.ToTable("Medicines"));
            modelBuilder.Entity<MomoPayments>(x => x.ToTable("MomoPayments"));
            modelBuilder.Entity<NotificationTemplates>(x => x.ToTable("NotificationTemplates"));
            modelBuilder.Entity<AllergyTypes>(x => x.ToTable("AllergyTypes"));
            modelBuilder.Entity<VaccineTypes>(x => x.ToTable("VaccineTypes"));
            modelBuilder.Entity<HospitalTypes>(x => x.ToTable("HospitalTypes"));
            modelBuilder.Entity<HospitalFunctionTypes>(x => x.ToTable("HospitalFunctionTypes"));
            modelBuilder.Entity<AllergyDescriptionTypes>(x => x.ToTable("AllergyDescriptionTypes"));

            #endregion

            modelBuilder.Entity<AppointmentSchedules>(x => x.ToTable("AppointmentSchedules"));
            //modelBuilder.Entity<ChannelMappingHospital>(x => x.ToTable("ChannelMappingHospital"));

            modelBuilder.Entity<Doctors>(x => x.ToTable("Doctors"));
            modelBuilder.Entity<DoctorDetails>(x => x.ToTable("DoctorDetails"));
            modelBuilder.Entity<ExaminationForms>(x => x.ToTable("ExaminationForms"));
            modelBuilder.Entity<ExaminationSchedules>(x => x.ToTable("ExaminationSchedules"));
            modelBuilder.Entity<ExaminationScheduleDetails>(x => x.ToTable("ExaminationScheduleDetails"));
            modelBuilder.Entity<Hospitals>(x => x.ToTable("Hospitals"));
            modelBuilder.Entity<HospitalFiles>(x => x.ToTable("HospitalFiles"));

            modelBuilder.Entity<MedicalRecords>(x => x.ToTable("MedicalRecords"));
            modelBuilder.Entity<MedicalRecordAdditions>(x => x.ToTable("MedicalRecordAdditions"));
            //modelBuilder.Entity<ServiceTypeMappingHospital>(x => x.ToTable("ServiceTypeMappingHospital"));

            modelBuilder.Entity<ExaminationHistories>(x => x.ToTable("ExaminationHistories"));
            modelBuilder.Entity<BankInfos>(x => x.ToTable("BankInfos"));
            modelBuilder.Entity<PaymentHistories>(x => x.ToTable("PaymentHistories"));
            modelBuilder.Entity<ConfigRoomExaminations>(x => x.ToTable("ConfigRoomExaminations"));

            modelBuilder.Entity<PermitObjects>(x => x.ToTable("PermitObjects"));
            modelBuilder.Entity<Permissions>(x => x.ToTable("Permissions"));
            modelBuilder.Entity<UserGroups>(x => x.ToTable("UserGroups"));
            modelBuilder.Entity<PermitObjectPermissions>(x => x.ToTable("PermitObjectPermissions"));
            modelBuilder.Entity<UserInGroups>(x => x.ToTable("UserInGroups"));
            modelBuilder.Entity<NotificationApplicationUser>(x => x.ToTable("NotificationApplicationUser"));

            modelBuilder.Entity<MedicalRecordFiles>(x => x.ToTable("MedicalRecordFiles"));
            modelBuilder.Entity<MedicalRecordDetails>(x => x.ToTable("MedicalRecordDetails"));
            modelBuilder.Entity<MedicalRecordDetailFiles>(x => x.ToTable("MedicalRecordDetailFiles"));

            modelBuilder.Entity<UserFiles>(x => x.ToTable("UserFiles"));
            modelBuilder.Entity<ExaminationFormDetails>(x => x.ToTable("ExaminationFormDetails"));

            modelBuilder.Entity<OTPHistories>(x => x.ToTable("OTPHistories"));
            modelBuilder.Entity<SMSEmailTemplates>(x => x.ToTable("SMSEmailTemplates"));
            modelBuilder.Entity<ContentReplaceCharacters>(x => x.ToTable("ContentReplaceCharacters"));
            modelBuilder.Entity<SystemComments>(x => x.ToTable("SystemComments"));
            modelBuilder.Entity<UserFolders>(x => x.ToTable("UserFolders"));
            modelBuilder.Entity<MedicalRecordHistories>(x => x.ToTable("MedicalRecordHistories"));
            modelBuilder.Entity<ExaminationScheduleJobs>(x => x.ToTable("ExaminationScheduleJobs"));
            modelBuilder.Entity<UserAllergies>(x => x.ToTable("UserAllergies"));
            modelBuilder.Entity<NewFeeds>(x => x.ToTable("NewFeeds"));
            modelBuilder.Entity<UserPregnancies>(x => x.ToTable("UserPregnancies"));
            modelBuilder.Entity<UserPregnancyDetails>(x => x.ToTable("UserPregnancyDetails"));
            modelBuilder.Entity<SystemFiles>(x => x.ToTable("SystemFiles"));
            modelBuilder.Entity<DiagnoticTypes>(x => x.ToTable("DiagnoticTypes"));
            modelBuilder.Entity<HospitalHistories>(x => x.ToTable("HospitalHistories"));
            modelBuilder.Entity<UserVaccineProcesses>(x => x.ToTable("UserVaccineProcesses"));
            modelBuilder.Entity<VNPayPaymentHistories>(x => x.ToTable("VNPayPaymentHistories"));
            modelBuilder.Entity<ExaminationFormAdditionServiceMappings>(x => x.ToTable("ExaminationFormAdditionServiceMappings"));
            modelBuilder.Entity<ExaminationScheduleMappingUsers>(x => x.ToTable("ExaminationScheduleMappingUsers"));
            modelBuilder.Entity<AppPartners>(x => x.ToTable("AppPartners"));
            modelBuilder.Entity<AppPolicies>(x => x.ToTable("AppPolicies"));
            modelBuilder.Entity<AppPolicyDetails>(x => x.ToTable("AppPolicyDetails"));
            modelBuilder.Entity<UserAllergyDetails>(x => x.ToTable("UserAllergyDetails"));
            modelBuilder.Entity<VaccineTypeDetails>(x => x.ToTable("VaccineTypeDetails"));


            #region Configs

            modelBuilder.Entity<EmailConfiguration>(x => x.ToTable("EmailConfiguration"));
            modelBuilder.Entity<SMSConfiguration>(x => x.ToTable("SMSConfiguration"));
            modelBuilder.Entity<HospitalConfigFees>(x => x.ToTable("HospitalConfigFee"));
            modelBuilder.Entity<SystemConfiguartions>(x => x.ToTable("SystemConfiguartions"));
            modelBuilder.Entity<MomoConfigurations>(x => x.ToTable("MomoConfigurations"));
            modelBuilder.Entity<SystemConfigFee>(x => x.ToTable("SystemConfigFee"));
            modelBuilder.Entity<FaceBookAuthSettings>(x => x.ToTable("FaceBookAuthSettings"));
            modelBuilder.Entity<GoogleSettings>(x => x.ToTable("GoogleSettings"));
            modelBuilder.Entity<SystemAdvertisements>(x => x.ToTable("SystemAdvertisements"));
            modelBuilder.Entity<UserSystemExtensionPosts>(x => x.ToTable("UserSystemExtensionPosts"));
            modelBuilder.Entity<HospitalHolidayConfigs>(x => x.ToTable("HospitalHolidayConfigs"));
            modelBuilder.Entity<DeviceApps>(x => x.ToTable("DeviceApps"));


            #endregion


            modelBuilder.Entity<Users>(x =>
            {
                x.ToTable("Users");
            });


            base.OnModelCreating(modelBuilder);
        }

        #region Config

        public DbSet<HospitalHolidayConfigs> HospitalHolidayConfigs { get; set; }
        public DbSet<UserSystemExtensionPosts> UserSystemExtensionPosts { get; set; }
        public DbSet<SystemAdvertisements> SystemAdvertisements { get; set; }
        public DbSet<GoogleSettings> GoogleSettings { get; set; }
        public DbSet<FaceBookAuthSettings> FaceBookAuthSettings { get; set; }
        public DbSet<SystemConfiguartions> SystemConfiguartions { get; set; }
        public DbSet<HospitalConfigFees> HospitalConfigFee { get; set; }
        public DbSet<EmailConfiguration> EmailConfiguration { get; set; }
        public DbSet<SMSConfiguration> SMSConfiguration { get; set; }
        public DbSet<MomoConfigurations> MomoConfigurations { get; set; }
        public DbSet<SystemConfigFee> SystemConfigFee { get; set; }

        #endregion

        #region Catalogue

        public DbSet<AllergyDescriptionTypes> AllergyDescriptionTypes { get; set; }
        public DbSet<HospitalTypes> HospitalTypes { get; set; }
        public DbSet<HospitalFunctionTypes> HospitalFunctionTypes { get; set; }
        public DbSet<AllergyTypes> AllergyTypes { get; set; }
        public DbSet<VaccineTypes> VaccineTypes { get; set; }
        public DbSet<NotificationTemplates> NotificationTemplates { get; set; }
        public DbSet<Medicines> Medicines { get; set; }
        public DbSet<MedicalBills> MedicalBills { get; set; }
        public DbSet<AdditionServices> AdditionServices { get; set; }
        public DbSet<AdditionServiceDetails> AdditionServiceDetails { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Relations> Relations { get; set; }
        public DbSet<UserInGroups> UserInGroups { get; set; }
        public DbSet<PermitObjects> PermitObjects { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<UserGroups> UserGroups { get; set; }
        public DbSet<PermitObjectPermissions> PermitObjectPermissions { get; set; }

        public DbSet<ExaminationHistories> ExaminationHistories { get; set; }
        public DbSet<BankInfos> BankInfos { get; set; }
        public DbSet<Channels> Channels { get; set; }
        public DbSet<Cities> Cities { get; set; }
        public DbSet<ConfigTimeExaminations> ConfigTimeExaminations { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<DegreeTypes> DegreeTypes { get; set; }
        public DbSet<Districts> Districts { get; set; }
        public DbSet<ExaminationTypes> ExaminationTypes { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Nations> Nations { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<NotificationTypes> NotificationTypes { get; set; }
        public DbSet<RoomExaminations> RoomExaminations { get; set; }
        public DbSet<ServiceTypes> ServiceTypes { get; set; }
        public DbSet<SessionTypes> SessionTypes { get; set; }
        public DbSet<SpecialistTypes> SpecialistTypes { get; set; }
        public DbSet<Wards> Wards { get; set; }

        #endregion

        public DbSet<VaccineTypeDetails> VaccineTypeDetails { get; set; }
        public DbSet<UserAllergyDetails> UserAllergyDetails { get; set; }
        public DbSet<AppPolicies> AppPolicies { get; set; }
        public DbSet<AppPolicyDetails> AppPolicyDetails { get; set; }
        
        public DbSet<AppPartners> AppPartners { get; set; }
        public DbSet<DeviceApps> DeviceApps { get; set; }
        public DbSet<ExaminationScheduleMappingUsers> ExaminationScheduleMappingUsers { get; set; }
        public DbSet<ExaminationFormAdditionServiceMappings> ExaminationFormAdditionServiceMappings { get; set; }
        public DbSet<VNPayPaymentHistories> VNPayPaymentHistories { get; set; }
        public DbSet<UserVaccineProcesses> UserVaccineProcesses { get; set; }
        public DbSet<HospitalHistories> HospitalHistories { get; set; }
        public DbSet<DiagnoticTypes> DiagnoticTypes { get; set; }
        public DbSet<SystemFiles> SystemFiles { get; set; }
        public DbSet<UserPregnancies> UserPregnancies { get; set; }
        public DbSet<UserPregnancyDetails> UserPregnancyDetails { get; set; }
        public DbSet<NewFeeds> NewFeeds { get; set; }
        public DbSet<UserAllergies> UserAllergies { get; set; }
        public DbSet<ExaminationScheduleJobs> ExaminationScheduleJobs { get; set; }
        public DbSet<UserFolders> UserFolders { get; set; }
        public DbSet<MedicalRecordHistories> MedicalRecordHistories { get; set; }

        public DbSet<SystemComments> SystemComments { get; set; }
        public DbSet<SMSEmailTemplates> SMSEmailTemplates { get; set; }
        public DbSet<ContentReplaceCharacters> ContentReplaceCharacters { get; set; }

        public DbSet<OTPHistories> OTPHistories { get; set; }
        public DbSet<MomoPayments> MomoPayments { get; set; }
        public DbSet<ExaminationFormDetails> ExaminationFormDetails { get; set; }

        public DbSet<MedicalRecordDetails> MedicalRecordDetails { get; set; }
        public DbSet<MedicalRecordDetailFiles> MedicalRecordDetailFiles { get; set; }


        public DbSet<MedicalRecordFiles> MedicalRecordFiles { get; set; }
        public DbSet<UserFiles> UserFiles { get; set; }

        public DbSet<NotificationApplicationUser> NotificationApplicationUser { get; set; }
        public DbSet<ConfigRoomExaminations> ConfigRoomExaminations { get; set; }
        public DbSet<PaymentHistories> PaymentHistories { get; set; }

        public DbSet<AppointmentSchedules> AppointmentSchedules { get; set; }
        public DbSet<ChannelMappingHospital> ChannelMappingHospital { get; set; }
        public DbSet<Doctors> Doctors { get; set; }
        public DbSet<DoctorDetails> DoctorDetails { get; set; }
        public DbSet<ExaminationForms> ExaminationForms { get; set; }
        public DbSet<ExaminationSchedules> ExaminationSchedules { get; set; }
        public DbSet<ExaminationScheduleDetails> ExaminationScheduleDetails { get; set; }
        public DbSet<Hospitals> Hospitals { get; set; }
        public DbSet<MedicalRecords> MedicalRecords { get; set; }
        public DbSet<MedicalRecordAdditions> MedicalRecordAdditions { get; set; }
        public DbSet<ServiceTypeMappingHospital> ServiceTypeMappingHospital { get; set; }
        public DbSet<Users> Users { get; set; }






        public DbQuery<TQuery> Query<TQuery>() where TQuery : class
        {
            throw new NotImplementedException();
        }
    }
}
