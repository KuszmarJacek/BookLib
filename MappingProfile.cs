using AutoMapper;
using BookLib.DTOs;
using BookLib.Entities;

namespace BookLib
{
    public class MappingProfile : Profile
    {   
        public MappingProfile() 
        {
            CreateMap<UserForRegistrationDTO, User>();
        }
    }
}
