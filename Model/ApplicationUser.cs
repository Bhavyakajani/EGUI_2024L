using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace test.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Discriminator { get; set; }
        public string? FirstName {  get; set; }

        public string? LastName { get; set; }

        public static implicit operator ApplicationUser?(EmployeeEdit? v)
        {
            throw new NotImplementedException();
        }
    }
}
