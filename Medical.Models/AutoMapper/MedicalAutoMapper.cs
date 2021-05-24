using AutoMapper;
using Medical.Entities;
using Medical.Entities.Extensions;
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

            CreateMap<UserModel, Users>().ReverseMap();
            CreateMap<PagedList<UserModel>, PagedList<Users>>().ReverseMap();

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

            #region Configuration

            CreateMap<EmailConfigurationModel, EmailConfiguration>().ReverseMap();

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
