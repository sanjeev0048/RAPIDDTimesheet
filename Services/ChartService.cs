using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScottPlot;
using RAPIDDTimesheet.Models;

namespace RAPIDDTimesheet.Services;

public class ChartService
{
    public void GeneratePieChart(IEnumerable<EmployeeSummary> summaries, string outputFilePath, int width = 950, int height = 700)
    {
        var summaryList = summaries.ToList();
        if (summaryList.Count == 0)
        {
            Console.WriteLine("[ChartService] No employee summaries available to plot.");
            return;
        }

        // 1. Initialize ScottPlot 5.x Plot
        Plot plot = new();

        // 2. Build PieSlice collection
        var slices = summaryList.Select(emp => new PieSlice
        {
            Value = emp.TotalHours,
            Label = $"{emp.PercentageOfTotal:F1}%",
            LegendText = $"{emp.EmployeeName} ({emp.TotalHours:F1}h)"
        }).ToList();

        // 3. Add Pie to the plot
        var pie = plot.Add.Pie(slices);
        pie.ExplodeFraction = 0.04;
        pie.SliceLabelDistance = 0.65;

        // 4. Styling
        plot.ShowLegend();
        plot.Axes.Frameless();
        plot.HideGrid();

        // 5. Ensure output directory exists and save PNG
        var dir = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        plot.SavePng(outputFilePath, width, height);
    }
}
