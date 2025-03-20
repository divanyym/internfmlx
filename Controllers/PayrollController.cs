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

        // Baca data karyawan dari CSV
        private List<User> ReadUserData()
        {
            List<User> users = new List<User>();
            if (!System.IO.File.Exists(userFilePath)) return users;

            var lines = System.IO.File.ReadAllLines(userFilePath);
            foreach (var line in lines.Skip(1)) // Lewati header
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

        // Baca data payroll dari CSV
        private List<Payroll> ReadPayrollData()
{
    List<User> users = ReadUserData();  // Pastikan ini juga mengembalikan data
    List<Payroll> payrolls = new List<Payroll>();

    if (!System.IO.File.Exists(payrollFilePath))
    {
        Console.WriteLine("🚨 ERROR: File payroll.csv tidak ditemukan!");
        return payrolls;
    }

    var lines = System.IO.File.ReadAllLines(payrollFilePath);
    Console.WriteLine($"📂 Payroll File Loaded: {lines.Length} lines");

    foreach (var line in lines.Skip(1)) // Skip header
    {
        var parts = line.Split(',');
        Console.WriteLine($"📝 Reading Line: {line}");  // Tambahkan log ini

        if (parts.Length == 5)
        {
            try
            {
                int id = int.Parse(parts[0]);
                User? user = users.FirstOrDefault(u => u.Id == id);

                if (user != null)
                {
                    payrolls.Add(new Payroll
                    {
                        Id = id,
                        Name = user.Name,
                        Level = user.Level,
                        Date = DateTime.Parse(parts[2], CultureInfo.InvariantCulture),
                        TapIn = TimeSpan.Parse(parts[3]),
                        TapOut = TimeSpan.Parse(parts[4])
                    });

                    Console.WriteLine($"✅ Added Payroll: {id} - {user.Name} - {parts[2]} - {parts[3]} - {parts[4]}");
                }
                else
                {
                    Console.WriteLine($"⚠️ WARNING: User ID {id} tidak ditemukan di data user.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: Gagal parsing line: {line}");
                Console.WriteLine($"🔍 Exception: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"⚠️ WARNING: Format salah di line: {line}");
        }
    }

    Console.WriteLine($"📊 Total Payroll Records Loaded: {payrolls.Count}");
    return payrolls;
}


        // Menampilkan data payroll
        public IActionResult Index()
        {
            var payrolls = ReadPayrollData();

            Console.WriteLine("Payroll Data Loaded:");
            foreach (var p in payrolls)
            {
                Console.WriteLine($"{p.Id} - {p.Name} - {p.Date:yyyy-MM-dd} - {p.TapIn} - {p.TapOut}");
            }

            return View(payrolls);
        }

        // Form untuk menambahkan data payroll
        public IActionResult Add()
        {
            return View();
        }

        // Simpan data payroll baru
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

    payrolls.Add(new Payroll
    {
        Id = Id,
        Name = user.Name,
        Level = user.Level,
        Date = Date,
        TapIn = TapIn,
        TapOut = TapOut
    });

    // Simpan ke CSV dengan format lengkap
    using (var writer = new StreamWriter(payrollFilePath))
    {
        writer.WriteLine("Id,Name,Level,Date,TapIn,TapOut"); // ✅ Header lengkap
        foreach (var p in payrolls)
        {
            writer.WriteLine($"{p.Id},{p.Name},{p.Level},{p.Date:yyyy-MM-dd},{p.TapIn},{p.TapOut}"); // ✅ Simpan lengkap
        }
    }

    return RedirectToAction("Index");
}

    }}
