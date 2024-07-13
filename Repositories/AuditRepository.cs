namespace TravelApp.Repositories;

using System;
using System.Collections.Generic;
using System.Text.Json;

public class AuditRepository : IAudit
{
    public string Date { get; set; }
    public string Action { get; set; }
    public string ItemData { get; set; }
    private string? auditEntry = null;
    public List<string> _auditEntries = new();
    private const string auditFile = "AuditFile.json";
    private static string CurrentDate => DateTime.Now.ToString();

    public AuditRepository()
    {
        Date = CurrentDate;

        if (File.Exists(auditFile))
        {
            string json = File.ReadAllText(auditFile);
            _auditEntries = JsonSerializer.Deserialize<List<string>>(json)!;
        }
    }

    public List<string> ReadAuditFile()
    {
        if (!File.Exists(auditFile))
        {
            TextPainting(ConsoleColor.DarkRed, $"\n\tThis file does not exist!");
            ShowMessageSenderInfo();
        }

        return _auditEntries.ToList();
    }

    public void AddEntryToAuditFile()
    {
        auditEntry = $"| Date: {Date} | Action: {Action} | Item data: {ItemData} |\n";
        TextPainting(ConsoleColor.DarkCyan, $"\n\t\t\tEntry preview:\n\n\t{auditEntry}");
        ShowMessageSenderInfo();
        _auditEntries.Add(auditEntry);

        using (var writer = File.AppendText(auditFile))
        {
            writer.WriteLine(auditEntry);
        }
    }

    public void SaveAuditFile()
    {
        string json = JsonSerializer.Serialize(_auditEntries);
        File.WriteAllText(auditFile, json);
        TextPainting(ConsoleColor.DarkYellow, $"\n\tAuditFile saved!");
        ShowMessageSenderInfo();
    }

    private void ShowMessageSenderInfo()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\t(Message from:\t{this.GetType().Name})\n");
        Console.ResetColor();
    }

    private static void TextPainting(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}