using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models //services
{
    public class User
    {
        
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Level { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "Phone number cannot exceed 12 digits.")]
        public string? Phone { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public ICollection<EmployeeLog>? EmployeeLogs { get; set; }

        
    }

}