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
        private readonly string filePath = "wwwroot/payroll.csv";

        // Baca data dari CSV
        private List<Payroll> ReadPayrollData()
        {
            List<Payroll> payrolls = new List<Payroll>();
            if (!System.IO.File.Exists(filePath)) return payrolls;

            var lines = System.IO.File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Split(',');
                if (parts.Length == 5)
                {
                    payrolls.Add(new Payroll
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Date = DateTime.Parse(parts[2], CultureInfo.InvariantCulture),
                        TapIn = TimeSpan.Parse(parts[3]),
                        TapOut = TimeSpan.Parse(parts[4])
                    });
                }
            }
            return payrolls;
        }

        // Simpan data ke CSV
        private void WritePayrollData(List<Payroll> payrolls)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Id,Name,Date,TapIn,TapOut");
                foreach (var p in payrolls)
                {
                    writer.WriteLine($"{p.Id},{p.Name},{p.Date:yyyy-MM-dd},{p.TapIn},{p.TapOut}");
                }
            }
        }

        // Tampilkan data payroll
        public IActionResult Index()
        {
            var payrolls = ReadPayrollData();
            return View(payrolls);
        }

        // Simpan data baru dari form input
        [HttpPost]
        public IActionResult SavePayroll(string Name, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            var payrolls = ReadPayrollData();
            int newId = payrolls.Count > 0 ? payrolls.Max(p => p.Id) + 1 : 1;

            payrolls.Add(new Payroll
            {
                Id = newId,
                Name = Name,
                Date = Date,
                TapIn = TapIn,
                TapOut = TapOut
            });

            WritePayrollData(payrolls);
            return RedirectToAction("Index");
        }
    }
}
