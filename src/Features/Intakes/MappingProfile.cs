using AutoMapper;
using IntakeApi.Entities;

namespace IntakeApi.Features.Intakes
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<IntakeEntity, Get.Request>();
            CreateMap<Post.Request, IntakeEntity>();
        }
    }
}