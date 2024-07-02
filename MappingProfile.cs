using AutoMapper;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Description_Role, opt => opt.MapFrom(src => src.description_role));
        CreateMap<RoleDto, Role>()
            .ForMember(dest => dest.description_role, opt => opt.MapFrom(src => src.Description_Role));

        CreateMap<RoleCreateDto, Role>()
            .ForMember(dest => dest.IdRole, opt => opt.Ignore());

        // Service Mapping
        CreateMap<Service, ServiceDto>();
        CreateMap<ServiceDto, Service>();
        CreateMap<ServiceCreateDto, Service>()
            .ForMember(dest => dest.IdService, opt => opt.Ignore());

        // User Mapping
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.IdUsuario));
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore());
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore());
        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.IdUsuario, opt => opt.Ignore());
// Appointment Mapping
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.IdAppointment, opt => opt.MapFrom(src => src.IdAppointment))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Observation, opt => opt.MapFrom(src => src.Observation))
            .ForMember(dest => dest.Allergy, opt => opt.MapFrom(src => src.Allergy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.IdAppointment, opt => opt.Ignore())
            .ForMember(dest => dest.UserAppointments, opt => opt.Ignore())
            .ForMember(dest => dest.appointment_history, opt => opt.Ignore());
        CreateMap<AppointmentCreateDto, Appointment>()
            .ForMember(dest => dest.IdAppointment, opt => opt.Ignore())
            .ForMember(dest => dest.UserAppointments, opt => opt.Ignore())
            .ForMember(dest => dest.appointment_history, opt => opt.Ignore());

        // UserAppointment Mapping
        CreateMap<UserAppointment, UserAppointmentDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));
        CreateMap<UserAppointmentDto, UserAppointment>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Service, opt => opt.Ignore());
        CreateMap<UserAppointmentCreateDto, UserAppointment>()
            .ForMember(dest => dest.IdUser_Appointment, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Service, opt => opt.Ignore());

        // AppointmentHistory Mapping
        CreateMap<AppointmentHistory, AppointmentHistoryDto>()
            .ForMember(dest => dest.Appointment, opt => opt.MapFrom(src => src.Appointment)) // Incluye la cita
            .ForMember(dest => dest.IdAppointmentHistory, opt => opt.MapFrom(src => src.Idappointment_history))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Appointment_IdAppointment, opt => opt.MapFrom(src => src.Appointment_idappointment))
            .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src => src.Created_At))
            .ForMember(dest => dest.Updated_At, opt => opt.MapFrom(src => src.Updated_At));
        CreateMap<AppointmentHistoryDto, AppointmentHistory>()
            .ForMember(dest => dest.Idappointment_history, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore());
        CreateMap<AppointmentHistoryCreateDto, AppointmentHistory>()
            .ForMember(dest => dest.Idappointment_history, opt => opt.Ignore())
            .ForMember(dest => dest.Appointment, opt => opt.Ignore());

        // Image Mapping
        CreateMap<Image, ImageDto>();
        CreateMap<ImageDto, Image>();
        CreateMap<ImageCreateDto, Image>()
            .ForMember(dest => dest.IdImage, opt => opt.Ignore());
    }
}