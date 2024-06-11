using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using test.Data;
using test.Models;

namespace car_workshop.Controllers
{
    [ApiController, Route("TicketEdit")] 
    public class TicketEditController(UserManager<ApplicationUser> userManager, ApplicationDBContext context) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var tickets = context.Tickets.Include(x => x.AssignedEmployee).ToList();
            return Ok(tickets);
        }
        //GET
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Employees = new SelectList(await context.Users.ToListAsync(), "Id", "FullName");
            return Ok();
        }
        [HttpPost("CreateTicket/ticket")]
        public async Task<IActionResult> Create([FromBody]TicketEdit ticket)
        {
            if (ticket == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(ticket.Model) && !string.IsNullOrEmpty(ticket.Brand) && !string.IsNullOrEmpty(ticket.ProblemDescription) && !string.IsNullOrEmpty(ticket.RegistrationId))
            {
                // Create a new ticket
                ticket.State = TicketState.Created;
                await context.Tickets.AddAsync(ticket);
                await context.SaveChangesAsync();
                //TempData["Success"] = "Ticket Added Successfully";
                return RedirectToAction("Index");
            }
            return Ok(ticket);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await context.Tickets.Include(t => t.AssignedEmployee).FirstOrDefaultAsync(t => t.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Employees = new SelectList(await context.Users.ToListAsync(), "Id", "FirstName");
            return Ok(ticket);
        }

        [HttpPost("Edit/{ticket},{id}")]
        public async Task<IActionResult> Edit([FromBody]TicketEdit ticket, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(ticket.Model) && !string.IsNullOrEmpty(ticket.Brand) && !string.IsNullOrEmpty(ticket.ProblemDescription) && !string.IsNullOrEmpty(ticket.RegistrationId))
            {

                var existingTicket = context.Tickets.Find(id);
                if (existingTicket == null)
                {
                    return NotFound();
                }

                existingTicket.Brand = ticket.Brand;
                existingTicket.Model = ticket.Model;
                existingTicket.ProblemDescription = ticket.ProblemDescription;
                existingTicket.RegistrationId = ticket.RegistrationId;
                existingTicket.StartDateTime = ticket.StartDateTime;
                existingTicket.EndDateTime = ticket.EndDateTime;
                existingTicket.State = ticket.State;
                //if (ticket.AssignedEmployee?.Id != null)
                //{
                //    existingTicket.AssignedEmployee = (ApplicationUser?)await context.Users.FindAsync(ticket.AssignedEmployee.Id);
                //}

                context.Tickets.Update(existingTicket);
                await context.SaveChangesAsync();
                /*TempData["Success"] = "Ticket Modified Successfully";*/
                return RedirectToAction("Index");
            }
            return Ok(ticket);
        }


        [HttpGet("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await context.Tickets.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }


        // Ensure that only authenticated users can assign tickets
        //[Authorize]
        /*[HttpPost(Assign)]
        public async Task<IActionResult> AssignToSelf(int? id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var ticket = await context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.AssignedEmployee = user;
            ticket.State = TicketState.InProgress;

            context.Update(ticket);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }*/


        [HttpGet("GetIdDelete/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var ticketFromDB = context.Tickets.Find(id);
            if (ticketFromDB == null)
            {
                return NotFound();
            }
            return Ok(ticketFromDB);
        }

        //[Authorize(Roles = "Admin")]
        //POST

        [HttpDelete("Delete/{id}"), ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = context.Tickets.Find(id);

            if (obj == null)
            {
                return NotFound();
            }
            context.Tickets.Remove(obj);
            context.SaveChanges();
            //TempData["Success"] = "Ticket Removed Successfully";
            return RedirectToAction("Index");

        }

    }
}
