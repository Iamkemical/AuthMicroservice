using AuthMicroservice.API.Dtos;
using AuthMicroservice.Data.Entities;
using AutoMapper;

namespace AuthMicroservice.API.AutoMapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}