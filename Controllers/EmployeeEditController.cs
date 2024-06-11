using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using test.Data;
using test.Models;

namespace test.Controllers
{
    //[Authorize(Roles = "Admin, Employee")]
    [ApiController, Route("EmployeeEdit")]
    public class EmployeeEditController(UserManager<ApplicationUser> userManager, ApplicationDBContext context) : Controller
    {
        //private readonly ApplicationDBContext _context;

        //public EmployeeEditController(ApplicationDBContext context)
        //{
        //    _context = context;
        //}
        [HttpGet("GetEmployees")]
        public IActionResult Index()
        {
            IEnumerable<EmployeeEdit> objEmployeeList = context.Employees;
            return Ok(objEmployeeList);
        }

        //GET
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return Ok();
        }

        //POST
        [HttpPost("AddEmployee/{employee}")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeEdit employee)
        {
            // Get the currently logged-in user
            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                // Assign the UserId of the current user to the Employee
                employee.UserId = currentUser.Id;

                // Check if FirstName, LastName, Role, and WagePerHour are provided
                if (!string.IsNullOrEmpty(employee.FirstName) &&
                    !string.IsNullOrEmpty(employee.LastName) &&
                    !string.IsNullOrEmpty(employee.Role) &&
                    employee.WagePerHour != 0)
                {
                    // Add the Employee to the context and save changes
                    context.Employees.Add(employee);
                    context.SaveChanges();

                    //TempData["Success"] = "Employee Added Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle the case where required fields are missing
                    ModelState.AddModelError("", "First Name, Last Name, Role, and Wage Per Hour are required fields.");
                    return Ok(employee);
                }
            }
            else
            {
                // Handle the case where the current user is not found
                //TempData["Error"] = "Error: Current user not found.";
                return RedirectToAction("Index"); // Redirect to Index or handle accordingly
            }
        }

        //[Authorize(Roles = "Admin")]
        //GET
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var employeeFromDB = await context.Employees.FindAsync(id);
            if (employeeFromDB == null)
            {
                return NotFound();
            }
            var roles = context.Roles.Select(r => r.Name).ToList();

            // Create a SelectList for the roles
            var roleSelectList = new SelectList(roles);

            // Pass the SelectList to the view
            ViewBag.Roles = roleSelectList;

            // Get the currently logged-in user's ID
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                // Assign the UserID to the Employee being edited
                employeeFromDB.UserId = currentUser.Id;
            }

            return Ok(employeeFromDB);
        }

        //POST
        [HttpPut("Edit/employee")]
        //[ValidateAntiForgeryToken]
        public IActionResult Edit([FromBody]EmployeeEdit employee)
        {

            if (employee.FirstName == employee.Role)
            {
                ModelState.AddModelError("Role", "First Name and Role Cannot be Same");
            }

            if (employee.FirstName != null && employee.LastName != null && employee.Role != null && employee.WagePerHour != 0)
            {
                // Load the existing employee from the database
                var existingEmployee = context.Employees.Find(employee.Id);
                if (existingEmployee == null)
                {
                    return NotFound();
                }

                // Update only the allowed properties
                existingEmployee.FirstName = employee.FirstName;
                existingEmployee.LastName = employee.LastName;
                existingEmployee.WagePerHour = employee.WagePerHour;
                existingEmployee.Role = employee.Role;

                // Exclude UserId from being modified
                context.Entry(existingEmployee).Property(x => x.UserId).IsModified = false;

                context.SaveChanges();
                //TempData["Success"] = "Employee Details Edited Successfully";
                return RedirectToAction("Index");
                
            }
            return Ok(employee);


        }
        //[Authorize(Roles = "Admin")]
        //GET
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var employeeFromDB = context.Employees.Find(id);
            if (employeeFromDB == null)
            {
                return NotFound();
            }
            return Ok(employeeFromDB);
        }
        //[Authorize(Roles = "Admin")]
        //POST
        [HttpDelete("DeletePost/{id}"), ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = context.Employees.Find(id);

            if (obj== null)
            {
                return NotFound();
            }
            context.Employees.Remove(obj);
            context.SaveChanges();
            //TempData["Success"] = "Employee Removed Successfully";
            return RedirectToAction("Index");
            
        }
        //[Authorize(Roles = "Admin, Employee")]
        // GET: Employees/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            var roles = context.Roles.Select(r => r.Name).ToList();

            // Create a SelectList for the roles
            var roleSelectList = new SelectList(roles);

            // Pass the SelectList to the view
            ViewBag.Roles = roleSelectList;

            return Ok(employee);
        }
    }
}
