using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Pages.Employees
{
    public class EditModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty]
        public Employee Employee { get; set; } = default!;
        
        public SelectList Departments { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await context.Employees.FindAsync(id);
            
            if (Employee == null)
            {
                return NotFound();
            }
            
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

            context.Attach(Employee).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(Employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EmployeeExists(int id)
        {
            return context.Employees.Any(e => e.Id == id);
        }
    }
}