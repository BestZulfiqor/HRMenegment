using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Pages.Departments
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Department = await _context.Departments.FindAsync(id);
            if (Department == null)
                return NotFound();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Department = await _context.Departments.FindAsync(id);
            if (Department != null)
            {
                _context.Departments.Remove(Department);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}