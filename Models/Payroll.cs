using System;

namespace MvcMovie.Models
{
    public class Payroll
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TapIn { get; set; }
        public TimeSpan TapOut { get; set; }
        public double TotalHours => (TapOut - TapIn).TotalHours;
        public double Salary => TotalHours * 50000; // Rp 50.000 per jam
    }
}
