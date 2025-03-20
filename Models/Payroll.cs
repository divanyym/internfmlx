using System;

namespace MvcMovie.Models
{
    public class Payroll
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Level { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TapIn { get; set; }
        public TimeSpan TapOut { get; set; }

        public double TotalHours => (TapOut - TapIn).TotalHours;

        public double Salary 
        {
            get
            {
                double rate = Level switch
                {
                    "junior" => 50000,
                    "mid" => 75000,
                    "senior" => 100000,
                    _ => 50000
                };
                return TotalHours * rate;
            }
        }
    }
}
