using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<Employee> Employees { get; set; } = [];
    }
}
