using MvcMovie.Models;
using MvcMovie.Strategies;

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
            .Select(p => new PayrollDTO
            {
                Id = p.Id,
                Name = p.Name,
                Level = p.Level,
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
            var user = _context.EmployeeLogs.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return ("Error", "User tidak ditemukan!");

            double totalHours = (tapOut - tapIn).TotalHours;
            var strategy = PayrollStrategyFactory.GetStrategy(user.Level);
            double totalSalary = strategy.CalculateSalary(totalHours);

            var newPayroll = new EmployeeLog
            {
                Id = user.Id,
                Date = date,
                TapIn = date.Date +tapIn,
                TapOut = date.Date +tapOut,
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
