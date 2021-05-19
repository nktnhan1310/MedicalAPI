﻿using Medical.Entities;
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

            #endregion

            modelBuilder.Entity<AppointmentSchedules>(x => x.ToTable("AppointmentSchedules"));
            modelBuilder.Entity<ChannelMappingHospital>(x => x.ToTable("ChannelMappingHospital"));

            modelBuilder.Entity<Doctors>(x => x.ToTable("Doctors"));
            modelBuilder.Entity<DoctorDetails>(x => x.ToTable("DoctorDetails"));
            modelBuilder.Entity<ExaminationForms>(x => x.ToTable("ExaminationForms"));
            modelBuilder.Entity<ExaminationSchedules>(x => x.ToTable("ExaminationSchedules"));
            modelBuilder.Entity<ExaminationScheduleDetails>(x => x.ToTable("ExaminationScheduleDetails"));
            modelBuilder.Entity<Hospitals>(x => x.ToTable("Hospitals"));
            modelBuilder.Entity<HospitalFiles>(x => x.ToTable("HospitalFiles"));

            modelBuilder.Entity<MedicalRecords>(x => x.ToTable("MedicalRecords"));
            modelBuilder.Entity<MedicalRecordAdditions>(x => x.ToTable("MedicalRecordAdditions"));
            modelBuilder.Entity<ServiceTypeMappingHospital>(x => x.ToTable("ServiceTypeMappingHospital"));

            modelBuilder.Entity<ExaminationHistories>(x => x.ToTable("ExaminationHistories"));
            modelBuilder.Entity<BankInfos>(x => x.ToTable("BankInfos"));
            modelBuilder.Entity<PaymentHistories>(x => x.ToTable("PaymentHistories"));
            modelBuilder.Entity<ConfigRoomExaminations>(x => x.ToTable("ConfigRoomExaminations"));

            modelBuilder.Entity<PermitObjects>(x => x.ToTable("PermitObjects"));
            modelBuilder.Entity<Permissions>(x => x.ToTable("Permissions"));
            modelBuilder.Entity<UserGroups>(x => x.ToTable("UserGroups"));
            modelBuilder.Entity<PermitObjectPermissions>(x => x.ToTable("PermitObjectPermissions"));
            modelBuilder.Entity<UserInGroups>(x => x.ToTable("UserInGroups"));

            #region Configs

            modelBuilder.Entity<EmailConfiguration>(x => x.ToTable("EmailConfiguration"));
            modelBuilder.Entity<SMSConfiguration>(x => x.ToTable("SMSConfiguration"));

            #endregion


            modelBuilder.Entity<Users>(x =>
            {
                x.ToTable("Users");
            });


            base.OnModelCreating(modelBuilder);
        }

        #region Config

        public DbSet<EmailConfiguration> EmailConfiguration { get; set; }
        public DbSet<SMSConfiguration> SMSConfiguration { get; set; }


        #endregion

        #region Catalogue

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
