using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Длина должен быть между 3 и 50 символов", MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Длина должен быть между 3 и 50 символов", MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress] public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,}$")]
        public string? PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }

        [Range(500, double.MaxValue)] 
        public decimal Salary { get; set; }

        public int PositionId { get; set; }
        public Position? Position { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}