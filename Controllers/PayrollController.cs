using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using MvcMovie.Services;
using System.Globalization;

namespace MvcMovie.Controllers
{
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        public interface IPayrollService
        {
        IEnumerable<Payroll> GetPayrollData();
        IDictionary<string, IEnumerable<Payroll>> GroupPayrollByName(IEnumerable<Payroll> payrolls);
        (string, string) SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut);
    }
        

        // 📌 Menampilkan data payroll di halaman Index
        public IActionResult Index()
        {
            var payrolls = _payrollService.GetPayrollData();
            ViewBag.GroupedPayrolls = _payrollService.GroupPayrollByName(payrolls);
            return View(payrolls);
        }

        // 📌 Form untuk menambahkan data payroll
        public IActionResult Add()
        {
            TempData["PreviousPage"] = Request.Headers["Referer"].ToString();
            return View();
        }

        // 📌 Simpan data payroll ke CSV
        [HttpPost]
        public IActionResult SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            var result = _payrollService.SavePayroll(Id, Date, TapIn, TapOut);
            TempData[result.Item1] = result.Item2;
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
