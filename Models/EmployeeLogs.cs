using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class EmployeeLog
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Level { get; set; }
        public DateTime Date { get; set; }
        public DateTime TapIn { get; set; }
        public DateTime TapOut { get; set; }
        public double TotalHours { get; set; }
        public double TotalSalary { get; set; }

        public int UserId { get; set; }  // foreign key
        public User? User { get; set; }  // relasi navigasi
        }
}
