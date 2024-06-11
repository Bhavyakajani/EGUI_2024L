using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using test.Data;
using test.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace test.Controllers
{
    [ApiController, Route("Roles")]
    /*[Authorize(Roles = "Admin")]*/
    public class AppRolesController(RoleManager<IdentityRole> roleManager, ApplicationDBContext context) : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpGet("GetRoles")]
        //Listing all Roles: Admin, Employee
        public IActionResult GetRoles()
        {
            var roles = context.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult<List<ApplicationUser>>> GetUserId() {
            var result = await context.Users.ToListAsync();
            return Ok(result);
        }

        [HttpGet("getSpecificUser/{id}")]
        public async Task<ActionResult<List<ApplicationUser>>> GetUserByID(string id)
        {
            var result = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("getSpecificRole/{id}")]
        public async Task<ActionResult<List<ApplicationUser>>> GetRoleByID(string id)
        {
            var result = await context.Roles.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("CreateRoles")]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            //Checking for duplicates
            if (!(await _roleManager.RoleExistsAsync(model.Name)))
            {
                _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
        [HttpPut]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        // PUT: AppRoles/Edit/5
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] IdentityRole role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            var result = await _roleManager.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            result.Name = role.Name;
            result.NormalizedName = role.NormalizedName;


            context.Roles.Update(result);
            await context.SaveChangesAsync();

            return Ok(result);

            
        }

        // GET: AppRoles/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // POST: AppRoles/Delete/5
       
        [HttpDelete("DeleteConfirmed/{id}")]
        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RoleExists(string id)
        {
            return await _roleManager.RoleExistsAsync(id);
        }
    }
}
