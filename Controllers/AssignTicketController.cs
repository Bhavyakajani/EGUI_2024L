using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using test.Data;
using test.Models;

namespace test.Controllers
{

	[ApiController, Route("AssignTickets")]
	public class AssignTicketController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDBContext _context;



		public AssignTicketController(UserManager<ApplicationUser> userManager, ApplicationDBContext context)
		{
			_userManager = userManager;
			_context = context;
		}
		[HttpGet("GetTicketLoggedInUser")]
		public async Task<ActionResult> IndexAsync()
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			// Find the user based on the email address

			if (userEmail == null)
			{
				return BadRequest("Please login");
			}
			var user = await _userManager.FindByEmailAsync(userEmail);
			var tickets = _context.Tickets.Include(x => x.AssignedEmployee).Include(x => x.PartsUsed)
							.Where(x => (x.State == 0 && x.AssignedEmployee == null) || x.AssignedEmployee.Id == user.Id).ToList();
			return Ok(tickets);
		}
		[HttpGet("GetTicket")]
		public async Task<IActionResult> GetTickets()
		{
			var result = await _context.Tickets.ToListAsync();
			return Ok(result);
		}
		// GET: Ticket/Edit/5
		[HttpGet("Edit/{id}")]
		public IActionResult Edit(int? id)
		{
			if (id == 0 || id == null)
			{
				return Ok(new TicketEdit());
			}

			var ticket = _context.Tickets.Include(x => x.PartsUsed).Where(x => x.TicketId == id).FirstOrDefault();
			if (ticket.PartsUsed != null)
			{
				decimal totalPrice = 0;
				foreach (var part in ticket.PartsUsed)
				{
					totalPrice += part.Amount * part.UnitPrice;
				}
				ticket.PricePaid = totalPrice;
			}

			return Ok(ticket);
		}
		[HttpGet("CheckAvailableSlots")]
		public async Task<IActionResult> CheckAvailableSlots(int ticketId, DateTime StartDateTime, DateTime EndDateTime)
		{
			var userEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

			if (userEmail == null)
			{
				// Handle the case where no user is found with the provided email address
				return Json("User email not found.");
			}

			// Find the user based on the email address
			var user = await _userManager.FindByEmailAsync(userEmail);

			if (user == null)
			{
				// Handle the case where no user is found with the provided email address
				return Json("User not found.");
			}

			// Check if user does not have the same slot assigned already for another ticket
			var tickets = _context.Tickets
				.Include(x => x.AssignedEmployee)
				.Where(x => x.StartDateTime == StartDateTime && x.AssignedEmployee.Id == user.Id && x.TicketId != ticketId);

			foreach (var oldTicket in tickets)
			{
				if (oldTicket.StartDateTime == StartDateTime)
				{
					// Handle the case where the start slot is already assigned
					return Json("Start slot " + StartDateTime + " is already assigned. Change slot!");
				}
				if (oldTicket.EndDateTime == EndDateTime)
				{
					// Handle the case where the end slot is already assigned
					return Json("End slot " + EndDateTime + " is already assigned. Change slot!");
				}
				if (StartDateTime == EndDateTime)
				{
					return Json("Start Slot " + StartDateTime + " and End slot " + EndDateTime + " can't be same. Change slot!");
				}
			}

			// Handle the case where the slots are not assigned
			return Json("");
		}


		[HttpPost("SaveChanges")]
		public async Task<IActionResult> SaveChanges(TicketEdit ticket)
		{
			//var exTicket = _context.Tickets.Find(ticket.Id);
			//ticket.Brand = exTicket.Brand;
			//ticket.Model = exTicket.Model;
			//ticket.TicketDescription = exTicket.TicketDescription;

			if (ModelState.IsValid)
			{
				var username = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
				// Find the user based on the email address
				var user = await _userManager.FindByNameAsync(username);

				ticket.AssignedEmployee = user;

				// Attempt to update the ticket
				_context.Tickets.Update(ticket);
				await _context.SaveChangesAsync();
				return RedirectToAction("Index"); // Return success message
			}
			return BadRequest(/*"Edit", ticket*/);
		}

        [HttpGet("GetParts")]
        public async Task<IActionResult> GetParts()
        {
			Part parts = new Part();
            
            return Ok(parts);
        }

        [HttpGet("GetParts/{id}")]
		public IActionResult AddParts(int id)
		{
			var ticket = _context.Tickets.Include(x => x.PartsUsed).Where(x => x.TicketId == id).FirstOrDefault();
			return Ok(ticket);
		}
		[HttpPut("EditParts/{id}, {partId}")]

		public IActionResult EditParts(int id, int partId)
		{
			Part data = new Part();
			if (id != 0)
			{
				/*TempData["TicketId"] = id;
				TempData["PartId"] = partId;*/
				var ticket = _context.Tickets.Include(x => x.PartsUsed).Where(x => x.TicketId == id).FirstOrDefault();

				if (partId == 0 || partId == null)
				{
					if (ticket.PartsUsed == null)
					{
						return Ok(data);
					}
				}
				else
				{
					data = ticket.PartsUsed.Where(x => x.PartId == partId).FirstOrDefault();
					return Ok(data);
				}
			}
			return Ok(data);
		}

		[HttpPost("SaveParts/{ticketId},{part}")]
		public async Task<IActionResult> SaveParts(int ticketId, Part part)
		{
			
			int partId = part.PartId;
			var existingTicket = await _context.Tickets.Include(x => x.PartsUsed).FirstOrDefaultAsync(x => x.TicketId == ticketId);

			if (existingTicket == null)
			{
				// Handle error: Ticket not found
				return NotFound();
			}

			// If there are existing parts, update them
			if (partId == 0)
			{
				// Add new part
				part.PartId = partId;
				existingTicket.PartsUsed.Add(part);
			}
			else
			{
				// Update existing part
				part.PartId = partId;
				var existingPart = existingTicket.PartsUsed.FirstOrDefault(p => p.PartId == partId);
				if (existingPart != null)
				{
					existingPart.Name = part.Name;
					//existingPart.Description = part.Description;
					existingPart.Amount = part.Amount;
					existingPart.UnitPrice = part.UnitPrice;
				}
				else
				{
					// Handle error: Part not found
					return NotFound();
				}
			}

			existingTicket.PricePaid = existingTicket.PartsUsed.Sum(p => p.Amount * p.UnitPrice);

			// Save changes to the database
			_context.Tickets.Update(existingTicket);
			await _context.SaveChangesAsync();

			// Redirect to appropriate action
			return RedirectToAction("AddParts", new { id = existingTicket.TicketId });
		}


		[HttpDelete("Delete/{id},{partId}")]
		public async Task<IActionResult> DeletePart(int id, int partId)
		{
			try
			{
				// Retrieve the ticket from the database
				var ticket = _context.Tickets.Include(x => x.PartsUsed).Where(x => x.TicketId == id).FirstOrDefault();
				if (ticket == null)
				{
					return NotFound();
				}

				// Remove the ticket part from the database
				var ticketPart = ticket.PartsUsed.Where(x => x.PartId == partId).FirstOrDefault();
				ticket.PartsUsed.Remove(ticketPart);

				//_context.Tickets.Remove(ticket);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Ticket Part deleted successfully";
				// Return success message
				return View();
			}
			catch (Exception ex)
			{
				// Return error message
				return BadRequest(ex.Message);
			}
		}

	}
}