using AutoMapper;
using MyWebAPI.Core.Entities;
using MyWebAPI.Infrastructure.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.API.Extensions
{
    /// <summary>
    /// 实体映射
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostResource>()
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.LastModified));

            CreateMap<PostResource, Post>();
            CreateMap<PostAddResource, Post>();
            CreateMap<Post, PostAddResource>();

        }
    }
}
