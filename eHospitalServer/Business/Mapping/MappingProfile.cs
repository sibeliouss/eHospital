using AutoMapper;
using Entities.Concrete;
using Entities.Dtos;

namespace Business.Mapping;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserDto, User>()
            .ForMember(member => member.FirstName, options => options.MapFrom(src => src.FirstName.Trim()))
            .ForMember(member => member.LastName, options => options.MapFrom(src => src.LastName.Trim()))
            .ForMember(member => member.Email, options => options.MapFrom(src => src.Email!.ToLower().Trim()))
            .ForMember(member => member.Address, options => options.MapFrom(src => src.FullAddress.Trim()))
              .ForMember(member => member.UserName, options =>
                        options.MapFrom(src => src.UserName == null
                                        ? src.FirstName
                                            .Trim()
                                            .ToLower()
                                            .Replace(" ", "")
                                            .Replace("ğ", "g")
                                            .Replace("ş", "s")
                                            .Replace("ı", "i")
                                            .Replace("ö", "o")
                                            .Replace("ü", "u")
                                            .Replace("ç", "c")
                                        : src.UserName.Trim().ToLower()
                                        ));

        CreateMap<CreatePatientDto, User>()
            .ForMember(member => member.FirstName, options => options.MapFrom(src => src.FirstName.Trim()))
            .ForMember(member => member.LastName, options => options.MapFrom(src => src.LastName.Trim()))
            .ForMember(member => member.Email, options => options.MapFrom(src => src.Email!.ToLower().Trim()))
            .ForMember(member => member.Address, options => options.MapFrom(src => src.FullAddress.Trim()))
             .ForMember(member => member.UserName, options =>
                        options.MapFrom(src =>
                                    src.FirstName
                                            .Trim()
                                            .ToLower()
                                            .Replace(" ", "")
                                            .Replace("ğ", "g")
                                            .Replace("ş", "s")
                                            .Replace("ı", "i")
                                            .Replace("ö", "o")
                                            .Replace("ü", "u")
                                            .Replace("ç", "c")
                                            ));

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(member => member.StartDate, options =>
                        options.MapFrom(src => DateTime.SpecifyKind(src.StartDate, DateTimeKind.Utc)))
            .ForMember(member => member.EndDate, options =>
                        options.MapFrom(src => DateTime.SpecifyKind(src.EndDate, DateTimeKind.Utc)))
            ;
    }
}