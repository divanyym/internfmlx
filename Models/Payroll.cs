public class PayrollDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Level { get; set; }
    public DateTime Date { get; set; }
    public DateTime TapIn { get; set; }
    public DateTime TapOut { get; set; }
    public int UserId { get; set; } 

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
