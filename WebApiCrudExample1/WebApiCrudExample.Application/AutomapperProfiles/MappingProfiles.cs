using AutoMapper;
using WebApiCrudExample.Model;

namespace WebApiCrudExample.Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<PersonModel, PersonResponse>();
        CreateMap<PersonRequest, PersonModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(dest => dest.LastModifiedAtUtc, opt => opt.Ignore());
    }
}