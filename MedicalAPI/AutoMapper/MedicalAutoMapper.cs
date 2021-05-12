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

            #region Catalogue

            CreateMap<ServiceTypeModel, ServiceTypes>().ReverseMap();
            CreateMap<PagedList<ServiceTypeModel>, PagedList<ServiceTypes>>().ReverseMap();
            CreateMap<ChannelModel, Channels>().ReverseMap();
            CreateMap<PagedList<ChannelModel>, PagedList<Channels>>().ReverseMap();
            CreateMap<RoomExaminationModel, RoomExaminations>().ReverseMap();
            CreateMap<PagedList<RoomExaminationModel>, PagedList<RoomExaminations>>().ReverseMap();

            #endregion
        }
    }
}
