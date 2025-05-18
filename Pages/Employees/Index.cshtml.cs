using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HRManagement.Models;
using HRManagement.Data;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Pages.Employees
{
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty] 
        public List<string> AllDepartments { get; set; } = [];
        
        [BindProperty] 
        public List<string> AllPositions { get; set; } = [];

        [BindProperty] 
        public List<Employee> Employees { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public FilterModel Filters { get; set; } = new();
        
        public IActionResult OnGetExport()
{
    var employees = context.Employees
        .Include(e => e.Department)
        .Include(e => e.Position)
        .ToList();

    using var stream = new MemoryStream();

    using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
    {
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = new Body();

        // Заголовок документа
        var title = new Paragraph(
            new Run(new Text("Список сотрудников")));
        title.ParagraphProperties = new ParagraphProperties(
            new Justification() { Val = JustificationValues.Center },
            new SpacingBetweenLines() { After = "200" });
        body.Append(title);

        // Создание таблицы
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
            new TableCell(new Paragraph(new Run(new Text("ФИО")))),
            new TableCell(new Paragraph(new Run(new Text("Должность")))),
            new TableCell(new Paragraph(new Run(new Text("Отдел")))),
            new TableCell(new Paragraph(new Run(new Text("Зарплата")))),
            new TableCell(new Paragraph(new Run(new Text("Дата приема"))))
        );
        table.Append(headerRow);

        // Данные сотрудников
        foreach (var emp in employees)
        {
            var row = new TableRow();
            row.Append(
                new TableCell(new Paragraph(new Run(new Text($"{emp.LastName} {emp.FirstName}")))),
                new TableCell(new Paragraph(new Run(new Text(emp.Position?.Title ?? "")))),
                new TableCell(new Paragraph(new Run(new Text(emp.Department?.Name ?? "")))),
                new TableCell(new Paragraph(new Run(new Text($"{emp.Salary} руб.")))),
                new TableCell(new Paragraph(new Run(new Text(emp.HireDate.ToShortDateString()))))
            );
            table.Append(row);
        }

        body.Append(table);
        mainPart.Document.Append(body);
        mainPart.Document.Save();
    }

    stream.Seek(0, SeekOrigin.Begin);
    return File(stream.ToArray(), 
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "Сотрудники.docx");
}

        public async Task OnGetAsync()
        {
            AllDepartments = await context.Departments.Select(d => d.Name).ToListAsync();
            AllPositions = await context.Positions.Select(p => p.Title).ToListAsync();
            
            var query = context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .AsQueryable();

            if (!string.IsNullOrEmpty(Filters.FirstName))
            {
                query = Filters.FirstNameFilterType switch
                {
                    FilterType.Contains => query.Where(r => r.FirstName.Contains(Filters.FirstName)),
                    FilterType.NotEqual => query.Where(r => !r.FirstName.Contains(Filters.FirstName)),
                    _ => query.Where(r => r.FirstName == Filters.FirstName)
                };
            }

            if (!string.IsNullOrEmpty(Filters.LastName))
            {
                query = Filters.LastNameFilterType switch
                {
                    FilterType.Contains => query.Where(r => r.LastName.Contains(Filters.LastName)),
                    FilterType.NotEqual => query.Where(r => !r.LastName.Contains(Filters.LastName)),
                    _ => query.Where(r => r.LastName == Filters.LastName)
                };
            }
            
            if (!string.IsNullOrEmpty(Filters.Position))
            {
                query = Filters.PositionFilterType switch
                {
                    FilterType.Contains => query.Where(r => r.Position.Title.Contains(Filters.Position)),
                    FilterType.NotEqual => query.Where(r => !r.Position.Title.Contains(Filters.Position)),
                    _ => query.Where(r => r.Position.Title == Filters.Position)
                };
            }

            if (Filters.FromSalary.HasValue && Filters.ToSalary.HasValue)
            {
                query = query.Where(r => r.Salary >= Filters.FromSalary && r.Salary <= Filters.ToSalary);
            }
            
            if (Filters.SalaryValue.HasValue)
            {
                query = Filters.SalaryFilterType switch
                {
                    FilterType.GreaterThan => query.Where(r => r.Salary > Filters.SalaryValue),
                    FilterType.GreaterThanOrEqual => query.Where(r => r.Salary >= Filters.SalaryValue),
                    FilterType.LessThan => query.Where(r => r.Salary < Filters.SalaryValue),
                    FilterType.LessThanOrEqual => query.Where(r => r.Salary <= Filters.SalaryValue),
                    FilterType.NotEqual => query.Where(r => r.Salary != Filters.SalaryValue),
                    FilterType.Top10 => query.OrderByDescending(r => r.Salary).Take(10),
                    FilterType.AboveAverage => query.Where(r => r.Salary > context.Employees.Average(e => e.Salary)),
                    FilterType.BelowAverage => query.Where(r => r.Salary < context.Employees.Average(e => e.Salary)),
                    _ => query.Where(r => r.Salary == Filters.SalaryValue)
                };
            }
            
            if (Filters.HireDateValue.HasValue)
            {
                query = Filters.HireDateFilterType switch
                {
                    FilterType.GreaterThan => query.Where(r => r.HireDate > Filters.HireDateValue),
                    FilterType.GreaterThanOrEqual => query.Where(r => r.HireDate >= Filters.HireDateValue),
                    FilterType.LessThan => query.Where(r => r.HireDate < Filters.HireDateValue),
                    FilterType.LessThanOrEqual => query.Where(r => r.HireDate <= Filters.HireDateValue),
                    FilterType.NotEqual => query.Where(r => r.HireDate != Filters.HireDateValue),
                    _ => query.Where(r => r.HireDate == Filters.HireDateValue)
                };
            }
            
            if (Filters.DepartmentNames != null && Filters.DepartmentNames.Count > 0)
            {
                query = query.Where(e => e.Department != null && Filters.DepartmentNames.Contains(e.Department.Name));
            }

            Employees = await query.ToListAsync();
        }
    }

    public class FilterModel
    {
        public string? FirstName { get; set; }
        public FilterType FirstNameFilterType { get; set; } = FilterType.Equals;
        
        public string? LastName { get; set; }
        public FilterType LastNameFilterType { get; set; } = FilterType.Equals;
        
        public string? Position { get; set; }
        public FilterType PositionFilterType { get; set; } = FilterType.Equals;
        
        public decimal? SalaryValue { get; set; }
        public FilterType SalaryFilterType { get; set; } = FilterType.Equals;
        
        public decimal? FromSalary { get; set; }
        public decimal? ToSalary { get; set; }
        
        public DateTime? HireDateValue { get; set; }
        public FilterType HireDateFilterType { get; set; } = FilterType.Equals;
        
        public List<string> DepartmentNames { get; set; } = new();
    }

    public enum FilterType
    {
        Equals,
        Contains,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Top10,
        AboveAverage,
        BelowAverage
    }
}