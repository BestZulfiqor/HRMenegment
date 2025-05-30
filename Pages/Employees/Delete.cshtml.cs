using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Pages.Employees
{
    public class DeleteModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Employee = await context.Employees.FindAsync(id);

            if (Employee != null)
            {
                context.Employees.Remove(Employee);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}