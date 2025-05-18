using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HRManagement.Data;
using Position = HRManagement.Models.Position;

namespace HRManagement.Pages.Positions
{
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        public IList<Position> Positions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Positions = await context.Positions.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var position = await context.Positions.FindAsync(id);
            if (position != null)
            {
                context.Positions.Remove(position);
                await context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
        public IActionResult OnGetExport()
        {
            var positions = context.Positions.ToList();

            using var stream = new MemoryStream();

            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                // Заголовок документа
                var title = new Paragraph(
                    new Run(
                        new Text("Список должностей")));
                title.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { After = "200" });
                body.Append(title);

                // Таблица с данными
                var table = new Table();
                
                // Стили таблицы
                var tableProperties = new TableProperties(
                    new TableBorders(
                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                    ));

                table.AppendChild(tableProperties);

                // Заголовки таблицы
                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("Название")))),
                    new TableCell(new Paragraph(new Run(new Text("Мин. зарплата")))),
                    new TableCell(new Paragraph(new Run(new Text("Макс. зарплата"))))
                );
                table.Append(headerRow);

                // Данные должностей
                foreach (var pos in positions)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(pos.Title)))),
                        new TableCell(new Paragraph(new Run(new Text(pos.MinSalary.ToString())))),
                        new TableCell(new Paragraph(new Run(new Text(pos.MaxSalary.ToString())))));
                    table.Append(row);
                }

                body.Append(table);
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Должности.docx");
        }
    }
}