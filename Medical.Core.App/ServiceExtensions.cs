using Medical.Service;
using Medical.AppDbContext;
using Medical.Entities.DomainEntity;
using Medical.Interface;
using Medical.Interface.DbContext;
using Medical.Interface.Repository;
using Medical.Interface.Services;
using Medical.Interface.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Medical.Service.Services;

namespace Medical.Core.App
{
    public static class ServiceExtensions
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IMedicalDbContext, MedicalDbContext>();
            services.AddScoped(typeof(IDomainRepository<>), typeof(DomainRepository<>));
            services.AddScoped(typeof(ICatalogueRepository<>), typeof(CatalogueRepository<>));
            services.AddScoped(typeof(IMedicalRepository<>), typeof(MedicalRepository<>));
            services.AddScoped<IMedicalUnitOfWork, MedicalUnitOfWork>();
        }

        public static void ConfigureService(this IServiceCollection services)
        {
            services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("he")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddTransient<ITokenManagerService, TokenManagerService>();


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<IHospitalFileService, HospitalFileService>();
            services.AddScoped<IChannelMappingHospitalService, ChannelMappingHospitalService>();
            services.AddScoped<IServiceTypeMappingHospitalService, ServiceTypeMappingHospitalService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IDoctorDetailService, DoctorDetailService>();
            services.AddScoped<IExaminationScheduleService, ExaminationScheduleService>();
            services.AddScoped<IExaminationScheduleDetailService, ExaminationScheduleDetailService>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IMedicalRecordAdditionService, MedicalRecordAdditionService>();
            services.AddScoped<IConfigRoomExaminationService, ConfigRoomExaminationService>();
            services.AddScoped<IExaminationFormService, ExaminationFormService>();
            services.AddScoped<IPaymentHistoryService, PaymentHistoryService>();
            services.AddScoped<IExaminationHistoryService, ExaminationHistoryService>();

            #region PERMISSION SERVICE

            services.AddScoped<IPermitObjectPermissionService, PermitObjectPermissionService>();
            services.AddScoped<IPermitObjectService, PermitObjectService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserInGroupService, UserInGroupService>();

            #endregion

            #region Catalogue Service

            services.AddScoped<IExaminationTypeService, ExaminationTypeService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IRoomExaminationService, RoomExaminationService>();
            services.AddScoped<ISpecialListTypeService, SpecialListTypeService>();
            services.AddScoped<IDegreeTypeService, DegreeTypeService>();
            services.AddScoped<ISessionTypeService, SessionTypeService>();
            services.AddScoped<IConfigTimeExaminationService, ConfigTimeExaminationService>();




            #endregion

            #region Configuration

            services.AddScoped<IEmailConfigurationService, EmailConfigurationService>();

            #endregion


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Medical API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
                //var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                ////var xmlPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), xmlFile);

                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                foreach (var fi in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(fi.FullName);
                }

                c.EnableAnnotations();
            });


        }

        public static void MigrationDatabase(this IServiceProvider services, IConfiguration configuration)
        {
            using (var context = services.GetRequiredService<MedicalDbContext>())
            {
                context.Database.Migrate();
            }
        }


    }
}
