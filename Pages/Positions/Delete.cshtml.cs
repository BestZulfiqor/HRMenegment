using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HRManagement.Data;

namespace HRManagement.Pages.Positions
{
    public class DeleteModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty]
        public Position Position { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Position = await context.Positions.FirstOrDefaultAsync(m => m.Id == id);

            if (Position == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Position = await context.Positions.FindAsync(id);

            if (Position != null)
            {
                context.Positions.Remove(Position);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}