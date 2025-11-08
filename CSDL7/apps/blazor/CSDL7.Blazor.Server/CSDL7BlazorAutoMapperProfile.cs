using CSDL7.MasterService.Services.Dtos.Wards;
using CSDL7.MasterService.Services.Dtos.Districts;
using CSDL7.MasterService.Services.Dtos.Provinces;
using Volo.Abp.AutoMapper;
using CSDL7.MasterService.Services.Dtos.Departments;
using AutoMapper;

namespace CSDL7.Blazor.Server;

public class CSDL7BlazorAutoMapperProfile : Profile
{
    public CSDL7BlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<DepartmentDto, UpdateDepartmentDto>();

        CreateMap<ProvinceDto, ProvinceUpdateDto>();

        CreateMap<DistrictDto, DistrictUpdateDto>();

        CreateMap<WardDto, WardUpdateDto>();
    }
}