public class PayrollDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Level { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan TapIn { get; set; }
    public TimeSpan TapOut { get; set; }
    public string Status { get; set; } = "Pending";


    public double TotalHours { get; set; }  // Bisa disimpan di database
    public double TotalSalary { get; set; } // Bisa disimpan di database

    public void CalculatePayroll()
    {
        TotalHours = (TapOut - TapIn).TotalHours;
        TotalSalary = TotalHours * GetHourlyRate(Level);
    }

    private double GetHourlyRate(string? level)
    {
        return level switch
        {
            "Junior" => 50000,
            "Senior" => 100000,
            _ => 75000
        };
    }
}
