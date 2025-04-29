using AutoMapper;
using BusinessCardManager.Domain.Dtos;
using BusinessCardManager.Domain.Entities;

namespace BusinessCardManager.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BusinessCard, BusinessCardDto>()
                .ForMember(dest => dest.UserName, opt =>
                opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ReverseMap();
            CreateMap<CreateBusinessCardDto, BusinessCard>().ReverseMap();
            CreateMap<BusinessCardDto, CreateBusinessCardDto>();


            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>().ReverseMap();
            CreateMap<UpdateUserDto, User>().ReverseMap();
        }
    }
}
