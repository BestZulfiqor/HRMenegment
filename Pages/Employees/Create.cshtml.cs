using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Pages.Employees
{
    public class CreateModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty] public Employee Employee { get; set; } = new();

        public SelectList Departments { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Departments = new SelectList(await context.Departments.ToListAsync(),
                "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Departments = new SelectList(await context.Departments.ToListAsync(),
                    "Id", "Name");
                return Page();
            }

            Employee.HireDate = DateTime.Now.ToUniversalTime();
            context.Employees.Add(Employee);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}