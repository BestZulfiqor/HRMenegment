using System.ComponentModel.DataAnnotations;

namespace HRManagement.Models
{
    public class Position
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        private string _title;
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title 
        { 
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название должности не может быть пустым");
                
                _title = value.Trim();
            }
        }

        [StringLength(500, MinimumLength = 5)]
        public string? Description { get; set; }

        [Range(500, double.MaxValue)]
        public decimal MinSalary { get; set; }

        [Range(500, double.MaxValue)]
        private decimal _maxSalary;
        
        [Range(500, double.MaxValue)]
        public decimal MaxSalary
        {
            get => _maxSalary;
            set
            {
                if (value > MinSalary)
                {
                    _maxSalary = value;
                }
                else
                {
                    throw new ArgumentException("Max salary must be greater than min salary");
                }
            }
        }

        public List<Employee> Employees { get; set; } = [];
    }
}