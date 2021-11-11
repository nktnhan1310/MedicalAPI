﻿using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
using Medical.Entities.Reports;
using Medical.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Medical.Models.AutoMapper
{
    public class MedicalAutoMapper : Profile
    {
        public MedicalAutoMapper()
        {
            #region Extension

            CreateMap<UserFileExtensionModel, UserFileExtensions>().ReverseMap();
            CreateMap<PagedList<UserFileExtensionModel>, PagedList<UserFileExtensions>>().ReverseMap();

            CreateMap<UserFolderExtensionModel, UserFolderExtensions>().ReverseMap();
            CreateMap<PagedList<UserFolderExtensionModel>, PagedList<UserFolderExtensions>>().ReverseMap();

            #endregion

            CreateMap<ExaminationScheduleMappingUserModel, ExaminationScheduleMappingUsers>().ReverseMap();
            CreateMap<PagedList<ExaminationScheduleMappingUserModel>, PagedList<ExaminationScheduleMappingUsers>>().ReverseMap();

            CreateMap<ExaminationScheduleExtensionModel, ExaminationScheduleExtensions>().ReverseMap();
            CreateMap<UserGeneralInfoModel, UserGeneralInfo>().ReverseMap();

            CreateMap<ExaminationFormAdditionServiceMappingModel, ExaminationFormAdditionServiceMappings>().ReverseMap();
            CreateMap<PagedList<ExaminationFormAdditionServiceMappingModel>, PagedList<ExaminationFormAdditionServiceMappings>>().ReverseMap();

            CreateMap<HospitalHolidayConfigModel, HospitalHolidayConfigs>().ReverseMap();
            CreateMap<PagedList<HospitalHolidayConfigModel>, PagedList<HospitalHolidayConfigs>>().ReverseMap();

            CreateMap<VNPayPaymentHistoryModel, VNPayPaymentHistories>().ReverseMap();
            CreateMap<PagedList<VNPayPaymentHistoryModel>, PagedList<VNPayPaymentHistories>>().ReverseMap();

            CreateMap<UserVaccineProcessModel, UserVaccineProcesses>().ReverseMap();
            CreateMap<PagedList<UserVaccineProcessModel>, PagedList<UserVaccineProcesses>>().ReverseMap();

            CreateMap<HospitalHistoryModel, HospitalHistories>().ReverseMap();
            CreateMap<PagedList<HospitalHistoryModel>, PagedList<HospitalHistories>>().ReverseMap();

            CreateMap<SystemFileModel, SystemFiles>().ReverseMap();
            CreateMap<PagedList<SystemFileModel>, PagedList<SystemFiles>>().ReverseMap();

            CreateMap<UserPregnancyDetailModel, UserPregnancyDetails>().ReverseMap();
            CreateMap<PagedList<UserPregnancyDetailModel>, PagedList<UserPregnancyDetails>>().ReverseMap();

            CreateMap<UserPregnancyModel, UserPregnancies>().ReverseMap();
            CreateMap<PagedList<UserPregnancyModel>, PagedList<UserPregnancies>>().ReverseMap();

            CreateMap<UserModel, Users>().ReverseMap();
            CreateMap<PagedList<UserModel>, PagedList<Users>>().ReverseMap();

            CreateMap<UserAllergyModel, UserAllergies>().ReverseMap();
            CreateMap<PagedList<UserAllergyModel>, PagedList<UserAllergies>>().ReverseMap();

            CreateMap<HospitalModel, Hospitals>().ReverseMap();
            CreateMap<PagedList<HospitalModel>, PagedList<Hospitals>>().ReverseMap();

            CreateMap<ServiceTypeMappingHospitalModel, ServiceTypeMappingHospital>().ReverseMap();
            CreateMap<ChannelMappingHospitalModel, ChannelMappingHospital>().ReverseMap();
            CreateMap<HospitalFileModel, HospitalFiles>().ReverseMap();

            CreateMap<DoctorModel, Doctors>().ReverseMap();
            CreateMap<PagedList<DoctorModel>, PagedList<Doctors>>().ReverseMap();
            CreateMap<DoctorDetailModel, DoctorDetails>().ReverseMap();
            CreateMap<PagedList<DoctorDetailModel>, PagedList<DoctorDetails>>().ReverseMap();
            CreateMap<ExaminationScheduleModel, ExaminationSchedules>().ReverseMap();
            CreateMap<PagedList<ExaminationScheduleModel>, PagedList<ExaminationSchedules>>().ReverseMap();
            CreateMap<ConfigTimeExaminationDayOfWeekModel, ConfigTimeExaminationDayOfWeek>().ReverseMap();
            CreateMap<ConfigTimeExaminationBySessionModel, ConfigTimeExaminationBySession>().ReverseMap();

            CreateMap<ExaminationScheduleDetailModel, ExaminationScheduleDetails>().ReverseMap();

            CreateMap<MedicalRecordModel, MedicalRecords>().ReverseMap();
            CreateMap<PagedList<MedicalRecordModel>, PagedList<MedicalRecords>>().ReverseMap();
            CreateMap<MedicalRecordAdditionModel, MedicalRecordAdditions>().ReverseMap();
            CreateMap<ConfigRoomExaminationModel, ConfigRoomExaminations>().ReverseMap();
            CreateMap<PagedList<ConfigRoomExaminationModel>, PagedList<ConfigRoomExaminations>>().ReverseMap();

            CreateMap<ExaminationFormModel, ExaminationForms>().ReverseMap();
            CreateMap<PagedList<ExaminationFormModel>, PagedList<ExaminationForms>>().ReverseMap();

            CreateMap<ExaminationHistoryModel, ExaminationHistories>().ReverseMap();
            CreateMap<PagedList<ExaminationHistoryModel>, PagedList<ExaminationHistories>>().ReverseMap();

            CreateMap<PaymentHistoryModel, PaymentHistories>().ReverseMap();
            CreateMap<PagedList<PaymentHistoryModel>, PagedList<PaymentHistories>>().ReverseMap();
            CreateMap<UpdateExaminationStatusModel, UpdateExaminationStatus>().ReverseMap();

            CreateMap<BankInfoModel, BankInfos>().ReverseMap();
            CreateMap<PagedList<BankInfoModel>, PagedList<BankInfos>>().ReverseMap();

            CreateMap<NotificationModel, Notifications>().ReverseMap();
            CreateMap<PagedList<NotificationModel>, PagedList<Notifications>>().ReverseMap();

            CreateMap<NotificationTemplateModel, NotificationTemplates>().ReverseMap();
            CreateMap<PagedList<NotificationTemplateModel>, PagedList<NotificationTemplates>>().ReverseMap();

            CreateMap<UserFileModel, UserFiles>().ReverseMap();
            CreateMap<PagedList<UserFileModel>, PagedList<UserFiles>>().ReverseMap();

            CreateMap<MedicalRecordFileModel, MedicalRecordFiles>().ReverseMap();
            CreateMap<PagedList<MedicalRecordFileModel>, PagedList<MedicalRecordFiles>>().ReverseMap();

            CreateMap<MedicalRecordDetailModel, MedicalRecordDetails>().ReverseMap();
            CreateMap<PagedList<MedicalRecordDetailModel>, PagedList<MedicalRecordDetails>>().ReverseMap();

            CreateMap<MedicalRecordDetailFileModel, MedicalRecordDetailFiles>().ReverseMap();
            CreateMap<PagedList<MedicalRecordDetailFileModel>, PagedList<MedicalRecordDetailFiles>>().ReverseMap();

            CreateMap<MomoPaymentModel, MomoPayments>().ReverseMap();
            CreateMap<PagedList<MomoPaymentModel>, PagedList<MomoPayments>>().ReverseMap();

            CreateMap<ExaminationFormDetailModel, ExaminationFormDetails>().ReverseMap();
            CreateMap<PagedList<ExaminationFormDetailModel>, PagedList<ExaminationFormDetails>>().ReverseMap();

            CreateMap<SMSEmailTemplateModel, SMSEmailTemplates>().ReverseMap();
            CreateMap<PagedList<SMSEmailTemplateModel>, PagedList<SMSEmailTemplates>>().ReverseMap();

            CreateMap<ContentReplaceCharacterModel, ContentReplaceCharacters>().ReverseMap();
            CreateMap<PagedList<ContentReplaceCharacterModel>, PagedList<ContentReplaceCharacters>>().ReverseMap();

            CreateMap<OTPHistoryModel, OTPHistories>().ReverseMap();
            CreateMap<PagedList<OTPHistoryModel>, PagedList<OTPHistories>>().ReverseMap();

            CreateMap<SystemCommentModel, SystemComments>().ReverseMap();
            CreateMap<PagedList<SystemCommentModel>, PagedList<SystemComments>>().ReverseMap();

            CreateMap<UserFolderModel, UserFolders>().ReverseMap();
            CreateMap<PagedList<UserFolderModel>, PagedList<UserFolders>>().ReverseMap();

            CreateMap<MedicalRecordHistoryModel, MedicalRecordHistories>().ReverseMap();
            CreateMap<PagedList<MedicalRecordHistoryModel>, PagedList<MedicalRecordHistories>>().ReverseMap();

            CreateMap<NewFeedModel, NewFeeds>().ReverseMap();
            CreateMap<PagedList<NewFeedModel>, PagedList<NewFeeds>>().ReverseMap();

            #region REPORT

            CreateMap<ReportRevenueModel, ReportRevenue>().ReverseMap();
            CreateMap<PagedListReport<ReportRevenueModel>, PagedListReport<ReportRevenue>>().ReverseMap();

            CreateMap<ReportExaminationFormModel, ReportExaminationForm>().ReverseMap();
            CreateMap<PagedListReport<ReportExaminationFormModel>, PagedListReport<ReportExaminationForm>>().ReverseMap();

            CreateMap<ReportUserExaminationFormModel, ReportUserExaminationForm>().ReverseMap();
            CreateMap<PagedListReport<ReportUserExaminationFormModel>, PagedListReport<ReportUserExaminationForm>>().ReverseMap();


            #endregion

            #region Configuration

            CreateMap<UserSystemExtensionPostModel, UserSystemExtensionPosts>().ReverseMap();
            CreateMap<PagedList<UserSystemExtensionPostModel>, PagedList<UserSystemExtensionPosts>>().ReverseMap();

            CreateMap<SystemConfiguartionModel, SystemConfiguartions>().ReverseMap();
            CreateMap<MomoConfigurationModel, MomoConfigurations>().ReverseMap();
            CreateMap<EmailConfigurationModel, EmailConfiguration>().ReverseMap();
            CreateMap<SMSConfiguartionModel, SMSConfiguration>().ReverseMap();
            CreateMap<HospitalConfigFeeModel, HospitalConfigFees>().ReverseMap();
            CreateMap<PagedList<HospitalConfigFeeModel>, PagedList<HospitalConfigFees>>().ReverseMap();
            CreateMap<PagedList<SystemConfigFeeModel>, PagedList<SystemConfigFee>>().ReverseMap();
            CreateMap<SystemConfigFeeModel, SystemConfigFee>().ReverseMap();

            CreateMap<PagedList<SystemAdvertisementModel>, PagedList<SystemAdvertisements>>().ReverseMap();
            CreateMap<SystemAdvertisementModel, SystemAdvertisements>().ReverseMap();

            #endregion

            #region Auth

            CreateMap<UserGroupModel, UserGroups>().ReverseMap();
            CreateMap<PagedList<UserGroupModel>, PagedList<UserGroups>>().ReverseMap();

            CreateMap<PermissionModel, Permissions>().ReverseMap();
            CreateMap<PagedList<PermissionModel>, PagedList<Permissions>>().ReverseMap();

            CreateMap<UserInGroupModel, UserInGroups>().ReverseMap();
            CreateMap<PagedList<UserInGroupModel>, PagedList<UserInGroups>>().ReverseMap();

            CreateMap<PermitObjectModel, PermitObjects>().ReverseMap();
            CreateMap<PagedList<PermitObjectModel>, PagedList<PermitObjects>>().ReverseMap();

            CreateMap<PermitObjectPermissionModel, PermitObjectPermissions>().ReverseMap();
            CreateMap<PagedList<PermitObjectPermissionModel>, PagedList<PermitObjectPermissions>>().ReverseMap();

            #endregion

            #region Catalogue

            CreateMap<HospitalTypeModel, HospitalTypes>().ReverseMap();
            CreateMap<PagedList<HospitalTypeModel>, PagedList<HospitalTypes>>().ReverseMap();

            CreateMap<HospitalFunctionTypeModel, HospitalFunctionTypes>().ReverseMap();
            CreateMap<PagedList<HospitalFunctionTypeModel>, PagedList<HospitalFunctionTypes>>().ReverseMap();

            CreateMap<DiagnoticTypeModel, DiagnoticTypes>().ReverseMap();
            CreateMap<PagedList<DiagnoticTypeModel>, PagedList<DiagnoticTypes>>().ReverseMap();

            CreateMap<VaccineTypeModel, VaccineTypes>().ReverseMap();
            CreateMap<PagedList<VaccineTypeModel>, PagedList<VaccineTypes>>().ReverseMap();

            CreateMap<AllergyTypeModel, AllergyTypes>().ReverseMap();
            CreateMap<PagedList<AllergyTypeModel>, PagedList<AllergyTypes>>().ReverseMap();

            CreateMap<MedicineModel, Medicines>().ReverseMap();
            CreateMap<PagedList<MedicineModel>, PagedList<Medicines>>().ReverseMap();

            CreateMap<MedicalBillModel, MedicalBills>().ReverseMap();
            CreateMap<PagedList<MedicalBillModel>, PagedList<MedicalBills>>().ReverseMap();

            CreateMap<AdditionServiceModel, AdditionServices>().ReverseMap();
            CreateMap<PagedList<AdditionServiceModel>, PagedList<AdditionServices>>().ReverseMap();

            CreateMap<NotificationTypeModel, NotificationTypes>().ReverseMap();
            CreateMap<PagedList<NotificationTypeModel>, PagedList<NotificationTypes>>().ReverseMap();

            CreateMap<PaymentMethodModel, PaymentMethods>().ReverseMap();
            CreateMap<PagedList<PaymentMethodModel>, PagedList<PaymentMethods>>().ReverseMap();

            CreateMap<RelationModel, Relations>().ReverseMap();
            CreateMap<PagedList<RelationModel>, PagedList<Relations>>().ReverseMap();

            CreateMap<CountryModel, Countries>().ReverseMap();
            CreateMap<PagedList<CountryModel>, PagedList<Countries>>().ReverseMap();

            CreateMap<NationModel, Nations>().ReverseMap();
            CreateMap<PagedList<NationModel>, PagedList<Nations>>().ReverseMap();

            CreateMap<DistrictModel, Districts>().ReverseMap();
            CreateMap<PagedList<DistrictModel>, PagedList<Districts>>().ReverseMap();

            CreateMap<CityModel, Cities>().ReverseMap();
            CreateMap<PagedList<CityModel>, PagedList<Cities>>().ReverseMap();

            CreateMap<WardModel, Wards>().ReverseMap();
            CreateMap<PagedList<WardModel>, PagedList<Wards>>().ReverseMap();


            CreateMap<JobModel, Jobs>().ReverseMap();
            CreateMap<PagedList<JobModel>, PagedList<Jobs>>().ReverseMap();
            CreateMap<ExaminationTypeModel, ExaminationTypes>().ReverseMap();
            CreateMap<PagedList<ExaminationTypeModel>, PagedList<ExaminationTypes>>().ReverseMap();
            CreateMap<ServiceTypeModel, ServiceTypes>().ReverseMap();
            CreateMap<PagedList<ServiceTypeModel>, PagedList<ServiceTypes>>().ReverseMap();
            CreateMap<ChannelModel, Channels>().ReverseMap();
            CreateMap<PagedList<ChannelModel>, PagedList<Channels>>().ReverseMap();
            CreateMap<RoomExaminationModel, RoomExaminations>().ReverseMap();
            CreateMap<PagedList<RoomExaminationModel>, PagedList<RoomExaminations>>().ReverseMap();
            CreateMap<SpecialistTypeModel, SpecialistTypes>().ReverseMap();
            CreateMap<PagedList<SpecialistTypeModel>, PagedList<SpecialistTypes>>().ReverseMap();
            CreateMap<DegreeTypeModel, DegreeTypes>().ReverseMap();
            CreateMap<PagedList<DegreeTypeModel>, PagedList<DegreeTypes>>().ReverseMap();
            CreateMap<SessionTypeModel, SessionTypes>().ReverseMap();
            CreateMap<PagedList<SessionTypeModel>, PagedList<SessionTypes>>().ReverseMap();
            CreateMap<ConfigTimeExamniationModel, ConfigTimeExaminations>().ReverseMap();
            CreateMap<PagedList<ConfigTimeExamniationModel>, PagedList<ConfigTimeExaminations>>().ReverseMap();


            #endregion
        }
    }
}
