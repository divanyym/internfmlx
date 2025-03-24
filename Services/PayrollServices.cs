using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MvcMovie.Services
{
    public class PayrollService
    {
        private readonly string userFilePath = "wwwroot/data.csv";
        private readonly string payrollFilePath = "wwwroot/payroll.csv";

        public List<User> ReadUserData()
        {
            List<User> users = new List<User>();
            if (!File.Exists(userFilePath)) return users;

            var lines = File.ReadAllLines(userFilePath).Skip(1);
            foreach (var line in lines)
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

        public List<Payroll> ReadPayrollData()
        {
            List<Payroll> payrolls = new List<Payroll>();
            List<User> users = ReadUserData();

            if (!File.Exists(payrollFilePath) || File.ReadAllLines(payrollFilePath).Length <= 1)
                return payrolls;

            var lines = File.ReadAllLines(payrollFilePath).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 8)
                {
                    int id = int.Parse(parts[0]);
                    User? user = users.FirstOrDefault(u => u.Id == id);

                    if (user != null &&
                        DateTime.TryParse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
                        TimeSpan.TryParse(parts[4], out TimeSpan tapIn) &&
                        TimeSpan.TryParse(parts[5], out TimeSpan tapOut) &&
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

        public List<object> GroupPayrollData(List<Payroll> payrolls)
        {
            return payrolls.GroupBy(p => p.Name)
                .Select(g => new { Name = g.Key, TotalSalary = g.Sum(p => p.TotalSalary) })
                .ToList<object>();
        }

        public bool SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            var payrolls = ReadPayrollData();
            var users = ReadUserData();
            var user = users.FirstOrDefault(u => u.Id == Id);

            if (user == null) return false;

            double totalHours = (TapOut - TapIn).TotalHours;
            double hourlyRate = GetHourlyRate(user.Level ?? "Default");
            double totalSalary = totalHours * hourlyRate;

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

            using (var writer = new StreamWriter(payrollFilePath))
            {
                writer.WriteLine("Id,Name,Level,Date,TapIn,TapOut,TotalHours,TotalSalary");
                foreach (var p in payrolls)
                {
                    writer.WriteLine($"{p.Id},{p.Name},{p.Level},{p.Date:yyyy-MM-dd},{p.TapIn},{p.TapOut},{p.TotalHours.ToString(CultureInfo.InvariantCulture)},{p.TotalSalary.ToString(CultureInfo.InvariantCulture)}");
                }
            }
            return true;
        }

        private double GetHourlyRate(string level)
        {
            return level.ToLower() switch
            {
                "junior" => 50000,
                "mid" => 75000,
                "senior" => 100000,
                _ => 40000
            };
        }
    }
}
