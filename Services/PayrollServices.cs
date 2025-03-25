// PayrollService.cs
using MvcMovie.Models;
using System.Globalization;

namespace MvcMovie.Services
{
    public class PayrollService
    {
        private readonly string _userFilePath;
        private readonly string _payrollFilePath;

        public PayrollService(IConfiguration configuration)
        {
            _userFilePath = configuration["FilePaths:UserFile"] ?? throw new ArgumentNullException(nameof(configuration), "User file path is missing");
            _payrollFilePath = configuration["FilePaths:PayrollFile"] ?? throw new ArgumentNullException(nameof(configuration), "Payroll file path is missing");
        }

        public IEnumerable<User> ReadUserData()
        {
            var users = new List<User>();
            if (!File.Exists(_userFilePath)) return users;

            var lines = File.ReadAllLines(_userFilePath);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length == 7)
                {
                    users.Add(new User
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Level = parts[2],
                        Gender = parts[3],
                        Address = parts[4],
                        Phone = parts[5],
                        Email = parts[6]
                    });
                }
            }
            return users;
        }

        public IEnumerable<Payroll> ReadPayrollData()
        {
            var payrolls = new List<Payroll>();
            var users = ReadUserData();
            if (!File.Exists(_payrollFilePath) || File.ReadAllLines(_payrollFilePath).Length <= 1) return payrolls;

            var lines = File.ReadAllLines(_payrollFilePath);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length == 8 && int.TryParse(parts[0], out int id))
                {
                    var user = users.FirstOrDefault(u => u.Id == id);
                    if (user != null && DateTime.TryParse(parts[3], out DateTime date) &&
                        TimeSpan.TryParse(parts[4], out TimeSpan tapIn) && TimeSpan.TryParse(parts[5], out TimeSpan tapOut) &&
                        double.TryParse(parts[6], NumberStyles.Any, CultureInfo.InvariantCulture, out double totalHours) &&
                        double.TryParse(parts[7], NumberStyles.Any, CultureInfo.InvariantCulture, out double totalSalary))
                    {
                        payrolls.Add(new Payroll
                        {
                            Id = id,
                            Name = user.Name,
                            Level = user.Level,
                            Date = date,
                            TapIn = tapIn,
                            TapOut = tapOut,
                            TotalHours = totalHours,
                            TotalSalary = totalSalary
                        });
                    }
                }
            }
            return payrolls;
        }

        public void SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            var payrolls = ReadPayrollData().ToList();
            var user = ReadUserData().FirstOrDefault(u => u.Id == Id);
            if (user == null) throw new Exception("User tidak ditemukan!");

            double totalHours = (TapOut - TapIn).TotalHours;
            double totalSalary = totalHours * GetHourlyRate(user.Level ?? "Default");

            payrolls.Add(new Payroll
            {
                Id = Id,
                Name = user.Name,
                Level = user.Level,
                Date = Date,
                TapIn = TapIn,
                TapOut = TapOut,
                TotalHours = totalHours,
                TotalSalary = totalSalary
            });

            using (var writer = new StreamWriter(_payrollFilePath))
            {
                writer.WriteLine("Id,Name,Level,Date,TapIn,TapOut,TotalHours,TotalSalary");
                foreach (var p in payrolls)
                {
                    writer.WriteLine($"{p.Id},{p.Name},{p.Level},{p.Date:yyyy-MM-dd},{p.TapIn},{p.TapOut},{p.TotalHours.ToString(CultureInfo.InvariantCulture)},{p.TotalSalary.ToString(CultureInfo.InvariantCulture)}");
                }
            }
        }

        private double GetHourlyRate(string level)
        {
            return level.ToLower() switch
            {
                "junior" => 50000,
                "mid" => 75000,
                "senior" => 100000,
                _ => 40000,
            };
        }
        
    }
}