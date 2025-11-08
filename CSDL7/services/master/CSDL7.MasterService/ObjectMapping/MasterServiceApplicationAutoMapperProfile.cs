using CSDL7.MasterService.Services.Dtos.Wards;
using CSDL7.MasterService.Entities.Wards;
using CSDL7.MasterService.Services.Dtos.Districts;
using CSDL7.MasterService.Entities.Districts;
using CSDL7.MasterService.Services.Dtos.Provinces;
using CSDL7.MasterService.Entities.Provinces;
using System;
using CSDL7.MasterService.Services.Dtos.Shared;
using CSDL7.MasterService.Services.Dtos.Departments;
using CSDL7.MasterService.Entities.Departments;
using Volo.Abp.AutoMapper;
using AutoMapper;
using CSDL7.MasterService.Entities.Commons;
using Volo.Abp.Application.Dtos;

namespace CSDL7.MasterService.ObjectMapping;

public class MasterServiceApplicationAutoMapperProfile : Profile
{
    public MasterServiceApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
        * Alternatively, you can split your mapping configurations
        * into multiple profile classes for a better organization. */
        CreateMap(typeof(PagedData<>), typeof(PagedResultDto<>));
        CreateMap<Department, DepartmentDto>();

        CreateMap<Province, ProvinceDto>();

        CreateMap<District, DistrictDto>();
        CreateMap<DistrictWithNavigationProperties, DistrictWithNavigationPropertiesDto>();
        CreateMap<Province, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Ward, WardDto>();
        CreateMap<WardWithNavigationProperties, WardWithNavigationPropertiesDto>();
        CreateMap<District, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    }
}