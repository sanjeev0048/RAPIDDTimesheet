using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RAPIDDTimesheet.Models;
using RAPIDDTimesheet.Services;

namespace RAPIDDTimesheet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=================================================================");
        Console.WriteLine(" RAPIDD Technologies - Employee Timesheet & Analytics System");
        Console.WriteLine(" .NET 9.0 | ScottPlot 5.1.59 | Production Execution");
        Console.WriteLine("=================================================================\n");

        // 1. Resolve API Endpoint securely (never hardcodes private credentials in source control)
        string? apiUrl = Environment.GetEnvironmentVariable("RAPIDD_API_URL");

        // Allow passing via command line argument as an alternative
        if (string.IsNullOrWhiteSpace(apiUrl) && args.Length > 0)
        {
            apiUrl = args[0];
        }

        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[Configuration Required] RAPIDD_API_URL environment variable is not set.");
            Console.WriteLine("Please set the environment variable in your terminal before running:");
            Console.WriteLine("  $env:RAPIDD_API_URL=\"<your-api-endpoint>\"");
            Console.WriteLine("Or pass it as an argument: dotnet run -- \"<your-api-endpoint>\"");
            Console.ResetColor();
            return;
        }

        List<TimeEntry>? entries = null;

        try
        {
            // 2. Fetch Time Entries from API
            Console.WriteLine("--> [Step 1] Connecting to RAPIDD Timesheet API...");
            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            // 3. Deserialize JSON
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            entries = JsonSerializer.Deserialize<List<TimeEntry>>(json, options);

            Console.WriteLine($"    Successfully retrieved {entries?.Count ?? 0} raw timesheet entries.");
        }
        catch (HttpRequestException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[HTTP Error] Failed to connect to API: {ex.Message}");
            Console.ResetColor();
            return;
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[JSON Error] Failed to parse API response: {ex.Message}");
            Console.ResetColor();
            return;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Unexpected Error]: {ex.Message}");
            Console.ResetColor();
            return;
        }

        if (entries == null || entries.Count == 0)
        {
            Console.WriteLine("No records were retrieved from the API.");
            return;
        }

        // 4. Data Processing & Hours Calculation using LINQ
        Console.WriteLine("\n--> [Step 2] Processing Employee Working Hours...");
        var analyticsService = new AnalyticsService();
        var summaries = analyticsService.ProcessSummaries(entries);

        Console.WriteLine($"    Processed {summaries.Count} unique employees.");
        Console.WriteLine("    ------------------------------------------------------------------");
        Console.WriteLine("    Rank | Employee Name         | Total Hours | Share   | Status");
        Console.WriteLine("    ------------------------------------------------------------------");

        int rank = 1;
        foreach (var emp in summaries)
        {
            string status = emp.IsBelowThreshold ? "< 100 hrs (WARN)" : "OK";
            Console.WriteLine($"     {rank,2}  | {emp.EmployeeName,-21} | {emp.TotalHours,8:F2}h | {emp.PercentageOfTotal,5:F1}% | {status}");
            rank++;
        }
        Console.WriteLine("    ------------------------------------------------------------------");

        // Setup output directory
        string baseDir = AppContext.BaseDirectory;
        string outputDir = Path.Combine(baseDir, "output");
        Directory.CreateDirectory(outputDir);

        // Also place in project root output folder
        string projectOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        Directory.CreateDirectory(projectOutputDir);

        // 5. Generate HTML Report
        Console.WriteLine("\n--> [Step 3] Generating HTML Timesheet Report...");
        var htmlService = new HtmlReportService();
        string html = htmlService.GenerateHtml(summaries, thresholdHours: 100.0);

        string htmlPath = Path.Combine(projectOutputDir, "employee_report.html");
        string htmlIndexPath = Path.Combine(projectOutputDir, "index.html");
        htmlService.SaveToFile(html, htmlPath);
        htmlService.SaveToFile(html, htmlIndexPath);
        Console.WriteLine($"    HTML Report saved to: {htmlPath}");

        // 6. Generate ScottPlot Pie Chart
        Console.WriteLine("\n--> [Step 4] Generating ScottPlot 5.1.59 Pie Chart PNG...");
        var chartService = new ChartService();
        string pngPath = Path.Combine(projectOutputDir, "employee_pie_chart.png");
        string pngAltPath = Path.Combine(projectOutputDir, "employee-hours.png");
        chartService.GeneratePieChart(summaries, pngPath);
        chartService.GeneratePieChart(summaries, pngAltPath);
        Console.WriteLine($"    Pie Chart PNG saved to: {pngPath}");

        Console.WriteLine("\n=================================================================");
        Console.WriteLine(" APPLICATION EXECUTION COMPLETE!");
        Console.WriteLine(" Generated Output Files:");
        Console.WriteLine($"   * HTML Report: {htmlPath}");
        Console.WriteLine($"   * Pie Chart:   {pngPath}");
        Console.WriteLine("=================================================================\n");
    }
}
