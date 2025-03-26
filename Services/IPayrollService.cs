namespace MvcMovie.Services
{
    public interface IPayrollService
    {
        // Mengelompokkan payroll berdasarkan nama karyawan
        IDictionary<string, IEnumerable<PayrollDTO>> GroupPayrollByName(IEnumerable<PayrollDTO> payrolls);

        // Membaca data payroll dari file
        IEnumerable<PayrollDTO> ReadPayrollData();

        // Menyimpan payroll berdasarkan parameter terpisah
        (string, string) SavePayroll(int Id, DateTime Date, TimeSpan TapIn, TimeSpan TapOut);
    }
}
