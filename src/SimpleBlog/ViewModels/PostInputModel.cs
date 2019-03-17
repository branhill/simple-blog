using AutoMapper;
using SimpleBlog.Models;
using SimpleBlog.SeedWorks;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SimpleBlog.ViewModels
{
    public class PostInputModel : Entity
    {
        public string Title { get; set; }

        [DisplayName("Save as draft")]
        public bool IsDraft { get; set; }

        [StringLength(1000)]
        public string Excerpt { get; set; }

        [Required]
        public string Content { get; set; }

        [DisplayName(nameof(Post.Category))]
        public int CategoryId { get; set; }

        public string Tags { get; set; }

        public static string TagPostsToString(IList<TagPost> tagPosts)
        {
            if (!tagPosts.Any())
                return string.Empty;

            return tagPosts
                 .Select(tp => tp.Tag.Name)
                 .Aggregate((sum, next) => sum + ", " + next);
        }

        public static IList<TagPost> StringToTagPosts(string tagString, IList<Tag> tags)
        {
            var tagNames = tagString.Split(',')
                .Select(tn => tn.Trim())
                .ToList();

            var result = new List<TagPost>(tagNames.Count);
            foreach (var tagName in tagNames)
            {
                var tag = tags.FirstOrDefault(t => t.Name == tagName)
                    ?? new Tag { Name = tagName };

                result.Add(new TagPost { Tag = tag });
            }

            return result;
        }

        public IList<TagPost> GetTagPosts(IList<Tag> tagList) =>
            StringToTagPosts(Tags, tagList);

        public class TagPostsToStringConverter : IValueConverter<IList<TagPost>, string>
        {
            public string Convert(IList<TagPost> sourceMember, ResolutionContext context) =>
                TagPostsToString(sourceMember);
        }
    }
}
