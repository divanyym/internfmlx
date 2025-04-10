namespace MvcMovie.Strategies
{
    public interface IPayrollStrategy
    {
        double CalculateSalary(double hours);
    }

    public class HourlyPayrollStrategy : IPayrollStrategy
    {
        public double CalculateSalary(double hours) => hours * 50000;
    }

    public class DailyPayrollStrategy : IPayrollStrategy
    {
        public double CalculateSalary(double hours) => 400000; // Flat per hari
    }

    public class DefaultPayrollStrategy : IPayrollStrategy
    {
        public double CalculateSalary(double hours) => hours * 40000;
    }

    public static class PayrollStrategyFactory
    {
        public static IPayrollStrategy GetStrategy(string level)
        {
            if (string.IsNullOrEmpty(level))
            {
                throw new ArgumentException("Level cannot be null or empty.", nameof(level));
            }

            
            return level.ToLower() switch
            {
                "junior" => new HourlyPayrollStrategy(),
                "mid" => new HourlyPayrollStrategy(),
                "senior" => new DailyPayrollStrategy(),
                _ => new DefaultPayrollStrategy(),
            };
        }
    }
}
