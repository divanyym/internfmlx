using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MvcMovie.Controllers
{
    public class PayrollController : Controller
    {
        private readonly string userFilePath = "wwwroot/data.csv";
        private readonly string payrollFilePath = "wwwroot/payroll.csv";

        // 📌 Baca data karyawan dari CSV
        private List<User> ReadUserData()
        {
            List<User> users = new List<User>();

            if (!System.IO.File.Exists(userFilePath))
            {
                Console.WriteLine("⚠️ WARNING: File data.csv tidak ditemukan!");
                return users;
            }

            var lines = System.IO.File.ReadAllLines(userFilePath);
            foreach (var line in lines.Skip(1)) // Skip header
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

        // 📌 Baca data payroll dari CSV
        private List<Payroll> ReadPayrollData()
        {
            List<Payroll> payrolls = new List<Payroll>();
            List<User> users = ReadUserData();

            if (!System.IO.File.Exists(payrollFilePath) || System.IO.File.ReadAllLines(payrollFilePath).Length <= 1)
            {
                Console.WriteLine("⚠️ WARNING: File payroll.csv kosong atau tidak ditemukan!");
                return payrolls;
            }

            var lines = System.IO.File.ReadAllLines(payrollFilePath);
            foreach (var line in lines.Skip(1)) // Skip header
            {
                var parts = line.Split(',');

                if (parts.Length == 8)
                {
                    try
                    {
                        int id = int.Parse(parts[0]);
                        User? user = users.FirstOrDefault(u => u.Id == id);

                        if (user != null)
                        {
                            if (DateTime.TryParse(parts[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) &&
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ ERROR: Gagal parsing line: {line}");
                        Console.WriteLine($"🔍 Exception: {ex.Message}");
                    }
                }
            }
            Console.WriteLine($"✅ Payroll yang terbaca: {payrolls.Count} data.");
            return payrolls;
        }

        // 📌 Menampilkan data payroll di halaman Index
        public IActionResult Index()
        {
            var payrolls = ReadPayrollData();
            var groupedPayrolls = payrolls
                .GroupBy(p => p.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalSalary = g.Sum(p => p.TotalSalary)
                }).ToList();

            ViewBag.GroupedPayrolls = groupedPayrolls;
            Console.WriteLine($"✅ Menampilkan {payrolls.Count} payrolls di halaman Index.");
            return View(payrolls);
        }

        // 📌 Form untuk menambahkan data payroll
        public IActionResult Add()
        {
            return View();
        }

        // 📌 Simpan data payroll ke CSV
        [HttpPost]
        public IActionResult SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            var payrolls = ReadPayrollData();
            var users = ReadUserData();
            var user = users.FirstOrDefault(u => u.Id == Id);

            if (user == null)
            {
                TempData["Error"] = "User tidak ditemukan!";
                return RedirectToAction("Add");
            }

            // Hitung total jam kerja
            TimeSpan duration = TapOut - TapIn;
            double totalHours = duration.TotalHours;

            // Format tampilan untuk debugging
            string formattedTotalHours = $"{(int)duration.TotalHours}h {duration.Minutes}m";
            Console.WriteLine($"⌛ Total Jam Kerja: {formattedTotalHours}");

            // Hitung total gaji berdasarkan level karyawan
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

            // Simpan ke CSV dengan format yang benar
            using (var writer = new StreamWriter(payrollFilePath))
            {
                writer.WriteLine("Id,Name,Level,Date,TapIn,TapOut,TotalHours,TotalSalary");
                foreach (var p in payrolls)
                {
                    writer.WriteLine($"{p.Id},{p.Name},{p.Level},{p.Date:yyyy-MM-dd},{p.TapIn},{p.TapOut},{p.TotalHours.ToString(CultureInfo.InvariantCulture)},{p.TotalSalary.ToString(CultureInfo.InvariantCulture)}");
                }
            }

            Console.WriteLine($"✅ Data payroll baru telah ditulis ke file. Total Payrolls: {payrolls.Count}");
            return RedirectToAction("Index");
        }

        // 📌 Fungsi untuk mendapatkan gaji per jam berdasarkan level karyawan
        private double GetHourlyRate(string level)
        {
            switch (level.ToLower())
            {
                case "junior":
                    return 50000; // Rp 50.000 per jam
                case "mid":
                    return 75000; // Rp 75.000 per jam
                case "senior":
                    return 100000; // Rp 100.000 per jam
                default:
                    return 40000; // Default jika level tidak ditemukan
            }
        }
    }
}
