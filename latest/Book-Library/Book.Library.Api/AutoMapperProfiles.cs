using AutoMapper;
using Book.Library.Api.ViewModels;
using Book.Library.Data.Entities;

namespace Book.Library.Api
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<BookEntity, BookVM>().ReverseMap();
        }
    }
}
