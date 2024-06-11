using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class EmployeeEdit
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string? FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string? LastName { get; set;}
        [Required]
        public string? Role { get; set; }
        [Required]
        [DisplayName("Wage Per Hour")]
        public decimal WagePerHour { get; set; }
    }
}
