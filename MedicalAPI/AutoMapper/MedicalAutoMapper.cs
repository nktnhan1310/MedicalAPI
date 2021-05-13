using AutoMapper;
using Medical.Entities;
using Medical.Utilities;
using MedicalAPI.Model;
using MedicalAPI.Model.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalAPI.AutoMapper
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

            #region Catalogue

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
