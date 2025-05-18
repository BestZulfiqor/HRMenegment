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
        public SelectList Positions { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await context.Employees
                .Include(e => e.Position)  // Добавляем загрузку должности
                .FirstOrDefaultAsync(e => e.Id == id);
            
            if (Employee == null)
            {
                return NotFound();
            }
            
            Departments = new SelectList(await context.Departments.ToListAsync(), "Id", "Name");
            Positions = new SelectList(await context.Positions.ToListAsync(), "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Получаем данные о выбранной должности
            var selectedPosition = await context.Positions
                .FirstOrDefaultAsync(p => p.Id == Employee.PositionId);

            // Проверяем соответствие зарплаты вилке должности
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

            var existingEmployee = await context.Employees.FindAsync(Employee.Id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            existingEmployee.FirstName = Employee.FirstName;
            existingEmployee.LastName = Employee.LastName;
            existingEmployee.Email = Employee.Email;
            existingEmployee.PhoneNumber = Employee.PhoneNumber;
            existingEmployee.Salary = Employee.Salary;
            existingEmployee.PositionId = Employee.PositionId;
            existingEmployee.DepartmentId = Employee.DepartmentId;

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
                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool EmployeeExists(int id)
        {
            return context.Employees.Any(e => e.Id == id);
        }
    }
}