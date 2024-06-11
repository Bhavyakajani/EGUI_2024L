using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class TicketEdit
    {
        [Key]
        public int? TicketId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegistrationId { get; set; }
        public string ProblemDescription { get; set; }
        public ApplicationUser? AssignedEmployee { get; set; }
        
        public TicketState State { get; set; }
        public List<Part> PartsUsed { get; set; }
        public decimal PricePaid { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }

    public enum TicketState
    {
        Created,
        InProgress,
        Done,
        Closed
    }

     public class Part
     {
        public int PartId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitPrice { get; set; }

     }
}
