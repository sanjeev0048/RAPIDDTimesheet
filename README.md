# RAPIDD Technologies – Employee Timesheet & Analytics System

An enterprise-grade C# .NET 9.0 console application that ingests employee timesheet records from the RAPIDD REST API, aggregates total working hours per employee using LINQ and TimeSpan arithmetic, generates a styled HTML report with conditional highlighting for employees below 100 hours, and produces a ScottPlot 5 pie chart PNG visualization.

---

## 🛠️ Technology Stack
- **Language**: C# 13 (.NET 9.0 SDK)
- **Runtime**: .NET 9.0 (`net9.0`)
- **HTTP Client**: `System.Net.Http.HttpClient`
- **JSON Engine**: `System.Text.Json` with `[JsonPropertyName]` attributes
- **Data Querying**: Language Integrated Query (LINQ)
- **HTML Composition**: `System.Text.StringBuilder` with responsive CSS
- **Charting Engine**: `ScottPlot 5.1.59`

---

## 📁 Project Architecture

```
RAPIDDTimesheet/
├── Models/
│   ├── TimeEntry.cs           # Maps raw JSON fields (StarTimeUtc typo handled)
│   └── EmployeeSummary.cs     # Aggregated hours, percentage share, threshold status
├── Services/
│   ├── AnalyticsService.cs    # Data validation, LINQ grouping, TimeSpan.TotalHours
│   ├── HtmlReportService.cs   # StringBuilder HTML generation with conditional CSS
│   └── ChartService.cs        # ScottPlot 5.1.59 pie chart & PNG export
├── Program.cs                 # Main async pipeline orchestrator
├── README.md                  # Project documentation
└── RAPIDDTimesheet.csproj     # .NET 9 project configuration
```

---

## 🚀 Build & Run Instructions

```powershell
cd C:\RAPIDDEmployeeTimesheet\RAPIDDTimesheet
dotnet build
dotnet run
```

### Publish as Standalone Windows Executable (.exe)
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```
Executable output: `bin\Release\net9.0\win-x64\publish\RAPIDDTimesheet.exe`
