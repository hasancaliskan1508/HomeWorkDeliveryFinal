using AutoMapper;
using HomeWorkDelivery.API.DTOs;
using HomeWorkDelivery.API.Models;

namespace HomeWorkDelivery.API.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<HomeWork, HomeWorkDto>().ReverseMap();
            CreateMap<HomeWorkStep, HomeWorkStepDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
        }
    }

}
