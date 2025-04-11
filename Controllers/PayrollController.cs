using Microsoft.AspNetCore.Mvc;
using MvcMovie.Services;

namespace MvcMovie.Controllers
{
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;
        private readonly ILogger<PayrollController> _logger;

        public PayrollController(IPayrollService payrollService, ILogger<PayrollController> logger)
        {
            _payrollService = payrollService;
            _logger = logger;
        }

        // 📌 Display payroll data on the Index page
        public IActionResult Index()
        {
            _logger.LogInformation("User accessed the payroll index page.");
            var payrolls = _payrollService.ReadPayrollData();
            ViewBag.GroupedPayrolls = _payrollService.GroupPayrollByName(payrolls);
            return View(payrolls);
        }

        // 📌 Form for adding payroll data
        public IActionResult Add()
        {
            _logger.LogInformation("User opened the payroll add form.");
            TempData["PreviousPage"] = Request.Headers["Referer"].ToString();
            return View();
        }

   
        [HttpPost]
        public IActionResult SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut)
        {
            try
            {
                _logger.LogInformation("User is attempting to save payroll data for ID {Id}.", Id);

                // Check if TapOut is earlier than TapIn
                if (TapOut < TapIn)
                {
                    _logger.LogWarning("TapOut time is earlier than TapIn for payroll ID {Id}.", Id);
                    TempData["Error"] = "TapOut cannot be earlier than TapIn.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                _payrollService.SavePayroll(Id, Date, TapIn, TapOut);
                _logger.LogInformation("Payroll data for ID {Id} has been successfully saved.", Id);
                TempData["Success"] = "Payroll data saved successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving payroll data for ID {Id}.", Id);
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
