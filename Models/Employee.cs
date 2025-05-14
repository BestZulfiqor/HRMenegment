using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Длина должен быть между 3 и 255 символов", MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(255, ErrorMessage = "Длина должен быть между 3 и 255 символов", MinimumLength = 3)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress] public string? Email { get; set; }

        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,}$")]
        public string? PhoneNumber { get; set; }

        public DateTime HireDate { get; set; }

        [Range(0, double.MaxValue)] public decimal Salary { get; set; }

        [MaxLength(50)] public string Position { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}