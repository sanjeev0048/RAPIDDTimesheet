namespace RAPIDDTimesheet.Models;

/// <summary>
/// Represents aggregated analytics and working hours for an individual employee.
/// </summary>
public class EmployeeSummary
{
    public string EmployeeName { get; set; } = string.Empty;
    public double TotalHours { get; set; }
    public double PercentageOfTotal { get; set; }
    public bool IsBelowThreshold => TotalHours < 100.0;
}
