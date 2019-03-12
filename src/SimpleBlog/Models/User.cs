using Microsoft.AspNetCore.Identity;

namespace SimpleBlog.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
