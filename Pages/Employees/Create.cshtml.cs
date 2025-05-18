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
        [BindProperty] 
        public Employee Employee { get; set; } = new();

        public SelectList Departments { get; set; }
        public SelectList Positions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Departments = new SelectList(await context.Departments.ToListAsync(), "Id", "Name");
            Positions = new SelectList(await context.Positions.ToListAsync(), "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var selectedPosition = await context.Positions
                .FirstOrDefaultAsync(p => p.Id == Employee.PositionId);

            if (selectedPosition != null && 
                (Employee.Salary < selectedPosition.MinSalary || 
                 Employee.Salary > selectedPosition.MaxSalary))
            {
                ModelState.AddModelError("Employee.Salary", 
                    $"Зарплата должна быть в диапазоне {selectedPosition.MinSalary} - {selectedPosition.MaxSalary}");
            }

            if (!ModelState.IsValid)
            {
                Departments = new SelectList(await context.Departments.ToListAsync(), "Id", "Name");
                Positions = new SelectList(await context.Positions.ToListAsync(), "Id", "Title");
                return Page();
            }

            Employee.HireDate = DateTime.Now.ToUniversalTime();
            context.Employees.Add(Employee);
            await context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}