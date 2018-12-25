using AutoMapper;
using BankofNeverland.IntakeApi.Entities;

namespace BankofNeverland.IntakeApi.Features.Intakes
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