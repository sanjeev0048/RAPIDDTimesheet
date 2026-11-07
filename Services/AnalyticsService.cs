using System;
using System.Collections.Generic;
using System.Linq;
using RAPIDDTimesheet.Models;

namespace RAPIDDTimesheet.Services;

public class AnalyticsService
{
    /// <summary>
    /// Filters out invalid records, groups by employee name, sums total working hours,
    /// sorts descending, and computes relative percentage of total hours.
    /// </summary>
    public List<EmployeeSummary> ProcessSummaries(IEnumerable<TimeEntry> entries)
    {
        if (entries == null)
        {
            return new List<EmployeeSummary>();
        }

        // 1. Filter out invalid/corrupt records:
        // - Missing or whitespace employee names
        // - Soft-deleted entries (where DeletedOn is populated)
        // - Inverted timestamps where EndTime <= StartTime
        var validEntries = entries.Where(e =>
            !string.IsNullOrWhiteSpace(e.EmployeeName) &&
            e.DeletedOn == null &&
            e.EndTimeUtc > e.StarTimeUtc
        );

        // 2. Group by employee and sum working hours using TotalHours
        var summaries = validEntries
            .GroupBy(e => e.EmployeeName!.Trim())
            .Select(group => new EmployeeSummary
            {
                EmployeeName = group.Key,
                TotalHours = group.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours)
            })
            // 3. Sort descending by total working hours
            .OrderByDescending(s => s.TotalHours)
            .ToList();

        // 4. Calculate employee percentages
        double grandTotal = summaries.Sum(s => s.TotalHours);
        if (grandTotal > 0)
        {
            foreach (var summary in summaries)
            {
                summary.PercentageOfTotal = (summary.TotalHours / grandTotal) * 100.0;
            }
        }

        return summaries;
    }
}
