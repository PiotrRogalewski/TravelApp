namespace TravelApp.Repositories
{
    public interface IAudit
    {
        string Date { get; }
        string Action { get; }
        string ItemData { get; }

        List<string> ReadAuditFile();
        void AddEntryToAuditFile();
        void SaveAuditFile();
    }
}