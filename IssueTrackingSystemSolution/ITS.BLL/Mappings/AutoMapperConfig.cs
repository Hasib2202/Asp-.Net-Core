using System;
using AutoMapper;
using ITS.DAL.Models;
using ITS.BLL.DTOs;

namespace ITS.BLL.Mappings
{
    public class AutoMapperConfig
    {
        public static Mapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDTO>().ReverseMap();

                cfg.CreateMap<Issue, IssueDTO>()
                    .ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.UserId));

                cfg.CreateMap<IssueDTO, Issue>()
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CreatedByUserId));

                cfg.CreateMap<Status, StatusDTO>().ReverseMap();
                cfg.CreateMap<Token, TokenDTO>()
                    .ForMember(dest => dest.TokenKey, opt => opt.MapFrom(src => src.Key))
                    .ReverseMap()
                    .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.TokenKey));
            });

            return new Mapper(config);
        }
    }
}
