namespace TravelApp.Repositories;

using System;
using System.Collections.Generic;
using System.Text.Json;

public class JsonAuditRepository : IAudit
{
    public string Date { get; set; }
    public string Action { get; set; }
    public string ItemData { get; set; }

    private string? auditEntry = null;

    public List<string> auditEntries = new();
    private const string? auditFile = "AuditFile.json";
    private static string currentDate => DateTime.Now.ToString();

    public JsonAuditRepository(string action, string itemData)
    {
        this.Date = currentDate;
        this.Action = action;
        this.ItemData = itemData;

        if (File.Exists(auditFile))
        {
            string json = File.ReadAllText(auditFile);
            auditEntries = JsonSerializer.Deserialize<List<string>>(json)!;
        }
    }

    public List<string> ReadAuditFile()
    {
        if (!File.Exists(auditFile))
        {
            TextPainting(ConsoleColor.DarkRed, "\tThis file does not exist!");
        }

        return auditEntries.ToList();
    }

    public void AddEntryToFile()
    {
        auditEntry = $"| Date: {Date} | Action: {Action} | Item data: {ItemData} |";
        TextPainting(ConsoleColor.DarkCyan, $"\n\t\t\tEntry preview:\n\n\t{auditEntry}");
        auditEntries.Add(auditEntry);

        using (var writer = File.AppendText(auditFile))
        {
            writer.WriteLine(auditEntry);
        }
    }

    public void SaveAuditFile()
    {
        string json = JsonSerializer.Serialize(auditEntries);
        File.WriteAllText(auditFile, json);
        TextPainting(ConsoleColor.DarkYellow, "\n\tAuditFile saved");
    }

    private static void TextPainting(ConsoleColor color, string text)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\t(Message from JsonAuditRepository)\n");
        Console.ResetColor();
    }
}