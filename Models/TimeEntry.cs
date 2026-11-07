using System;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Text.Json;

namespace RAPIDDTimesheet.Models;

public class TimeEntry
{
    public string Id { get; set; } = string.Empty;

    public string? EmployeeName { get; set; }

    [JsonPropertyName("StarTimeUtc")]
    public DateTime StarTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }

    public string? EntryNotes { get; set; }

    public DateTime? DeletedOn { get; set; }
}