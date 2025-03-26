using Microsoft.AspNetCore.Mvc;
using MvcMovie.Services;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;

        public PayrollController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        // 📌 Menampilkan data payroll di halaman Index
        public IActionResult Index()
        {
            var payrolls = _payrollService.ReadPayrollData();
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
            try
            {
                _payrollService.SavePayroll(Id, Date, TapIn, TapOut);
                TempData["Success"] = "Data payroll berhasil disimpan!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Terjadi kesalahan: {ex.Message}";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
