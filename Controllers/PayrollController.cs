using Microsoft.AspNetCore.Mvc;

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
            IEnumerable<PayrollDTO> GetPayrollData();
            IDictionary<string, IEnumerable<PayrollDTO>> GroupPayrollByName(IEnumerable<PayrollDTO> payrolls);
            (string, string) SavePayroll(PayrollDTO payroll);
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
        public IActionResult SavePayroll(PayrollDTO payroll)
        {
            var result = _payrollService.SavePayroll(payroll);
            TempData[result.Item1] = result.Item2;
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
