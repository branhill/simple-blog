using AutoMapper;
using SimpleBlog.Models;
using SimpleBlog.ViewModels;

namespace SimpleBlog.Infrastructures
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostInputModel>()
                .ForMember(d => d.Tags,
                    opt => opt.ConvertUsing(new PostInputModel.TagPostsToStringConverter(), s => s.TagPosts));
            CreateMap<PostInputModel, Post>();
        }
    }
}
