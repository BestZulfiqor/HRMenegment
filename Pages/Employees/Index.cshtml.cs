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
        public List<Employee> Employees { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public FilterModel Filters { get; set; } = new();

        public async Task OnGetAsync()
        {
            AllDepartments = await context.Departments.Select(d => d.Name).ToListAsync();
            var query = context.Employees.Include(r => r.Department).AsQueryable();

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
                    FilterType.Contains => query.Where(r => r.Position.Contains(Filters.Position)),
                    FilterType.NotEqual => query.Where(r => !r.Position.Contains(Filters.Position)),
                    _ => query.Where(r => r.Position == Filters.Position)
                };
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

            if (Filters.FromSalary.HasValue && Filters.ToSalary.HasValue)
            {
                query = query.Where(r => r.Salary >= Filters.FromSalary && r.Salary <= Filters.ToSalary);
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