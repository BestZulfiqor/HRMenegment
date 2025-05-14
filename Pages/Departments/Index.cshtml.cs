using Microsoft.AspNetCore.Mvc.RazorPages;
using HRManagement.Models;
using HRManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Department> Departments { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments.ToListAsync();
        }
    }
}