using AutoMapper;
using Music.Api.PostModels;
using Music.Api.PutModels;
using Music.Core.DTOs;

namespace Music.Api
{
    public class MappingPostProfile : Profile
    {
        public MappingPostProfile()
        {
            CreateMap<UserPostModel, UserDTO>();
            CreateMap<FilePostModel, FileDTO>();
            CreateMap<ConversionProcessPostModel, ConversionProcessDTO>();
            CreateMap<RolePostModel, RoleDTO>();
            CreateMap<UserPutModel, UserDTO>();
            CreateMap<FilePutModel, FileDTO>();
            CreateMap<LoginModel, UserDTO>();
        }
    }
}
