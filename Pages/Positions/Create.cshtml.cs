using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HRManagement.Models;
using HRManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Pages.Positions
{
    public class CreateModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty]
        public Position Position { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        private async Task<bool> PositionExistsAsync(string title)
        {
            return await context.Positions
                .AnyAsync(p => p.Title.ToLower() == title.Trim().ToLower());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверяем существование должности
            if (await PositionExistsAsync(Position.Title))
            {
                ModelState.AddModelError("Position.Title", "Должность с таким названием уже существует");
                return Page();
            }

            try
            {
                context.Positions.Add(Position);
                await context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Не удалось сохранить изменения. Попробуйте еще раз.");
                return Page();
            }
        }
    }
}