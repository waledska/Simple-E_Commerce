using Microsoft.AspNetCore.Identity;

namespace SimpleECommerce.DataAndContext.Models
{
    public class ApplicationUser : IdentityUser
    {
        // modify in the properties for the user here!
        public string TokenForRessetingPass { get; set; }

    }
}
