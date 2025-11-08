using System;
using CSDL7.EmailService.Services.Dtos.Shared;
using CSDL7.EmailService.Services.Dtos.Pings;
using CSDL7.EmailService.Entities.Pings;
using Volo.Abp.AutoMapper;
using AutoMapper;

namespace CSDL7.EmailService.ObjectMapping;

public class EmailServiceApplicationAutoMapperProfile : Profile
{
    public EmailServiceApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
        * Alternatively, you can split your mapping configurations
        * into multiple profile classes for a better organization. */

        CreateMap<Ping, PingDto>();
    }
}