using Microsoft.AspNetCore.Identity;

namespace HomeWorkDelivery.API.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string PicUrl { get; set; }
    }
}
