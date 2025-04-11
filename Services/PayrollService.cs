using MvcMovie.Models;
using MvcMovie.Strategies;
using Microsoft.EntityFrameworkCore;

namespace MvcMovie.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly AppDbContext _context;

        public PayrollService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<EmployeeLog> ReadUserData()
        {
            return _context.EmployeeLogs.ToList(); 
        }


        public IEnumerable<PayrollDTO> ReadPayrollData()
        {
           var payrolls = _context.EmployeeLogs
           .Include(p => p.User) // ← penting: join ke tabel Users
            .Select(p => new PayrollDTO
            {
                Id = p.Id,
                Name = p.User.Name,    // ← ambil dari User
                Level = p.User.Level,  // ← ambil dari User
                Date = p.Date,
                TapIn = p.TapIn,
                TapOut = p.TapOut,
                TotalHours = p.TotalHours,
                TotalSalary = p.TotalSalary
            })
            .ToList();

            return payrolls;
        }

        public (string, string) SavePayroll(int userId, DateTime date, TimeSpan tapIn, TimeSpan tapOut)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return ("Error", "User tidak ditemukan!");

            var dateUnspec = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            var tapInDateTime = DateTime.SpecifyKind(dateUnspec.Date + tapIn, DateTimeKind.Unspecified);
            var tapOutDateTime = DateTime.SpecifyKind(dateUnspec.Date + tapOut, DateTimeKind.Unspecified);

            double totalHours = (tapOut - tapIn).TotalHours;
            var strategy = PayrollStrategyFactory.GetStrategy(user.Level);
            double totalSalary = strategy.CalculateSalary(totalHours);

            var newPayroll = new EmployeeLog
            {
                UserId = user.Id,
                Date = dateUnspec,
                TapIn = tapInDateTime,
                TapOut = tapOutDateTime,
                TotalHours = totalHours,
                TotalSalary = totalSalary
            };
            
            _context.EmployeeLogs.Add(newPayroll);
            _context.SaveChanges();
            Console.WriteLine("Data berhasil ditambahkan ke database");


            return ("Success", "Payroll berhasil disimpan!");
        }

        public IDictionary<string, IEnumerable<PayrollDTO>> GroupPayrollByName(IEnumerable<PayrollDTO> payrolls)
        {
            return payrolls
                .GroupBy(p => p.Name ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }
    }
}
