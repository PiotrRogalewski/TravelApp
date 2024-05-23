namespace TravelApp.Repositories
{
    using System.Text.Json;
    using TravelApp.Entities;
    using System.Collections.Generic;

    public class JsonRepository<T> : IRepository<T>
        where T : class, IEntity, new()
    {
        private readonly Action<T>? _itemAddedCallback;
        private readonly List<T> items = new();
        private const string? emptyFile = "List_Of_";
        public string JsonFile { get; set; }



        public JsonRepository(string fileName, Action<T>? itemAddedCallback = null)
        {
            _itemAddedCallback = itemAddedCallback;
            JsonFile = emptyFile + fileName + ".json";

            if (File.Exists(JsonFile))
            {
                string json = File.ReadAllText(JsonFile);
                items = JsonSerializer.Deserialize<List<T>>(json)!;
            }
        }

        public event EventHandler<T>? ItemAdded;
        public event EventHandler<T>? ItemRemoved;
        public event EventHandler<T>? NewAuditEntry;

        public IEnumerable<T> GetAll()
        {
            return (IEnumerable<T>)items.ToList();
        }

        public T GetById(int id)
        {
            return items.Single(item => item.Id == id);
        }

        public void Add(T item)
        {
            item.Id = items.Count + 1;
            int newId = item.Id;

            for (newId = item.Id; newId < int.MaxValue; newId++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("\n\tChecking available ID numbers...");
                Console.ResetColor();

                item.Id = newId;

                if (items.Any(x => x.Id == item.Id))
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("\tId already taken. Looking for a new ID for this item\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\tSuccess! ID assigned\n");
                    Console.ResetColor();
                    item.Id = newId;
                    break;
                }
            }

            items.Add(item);

            using (var writer = File.AppendText(JsonFile))
            {
                writer.WriteLine(item);
            }

            _itemAddedCallback?.Invoke(item);
            ItemAdded?.Invoke(this, item);
            NewAuditEntry?.Invoke(this, item);
        }

        public void Remove(T item)
        {
            items.Remove(item);
            ItemRemoved?.Invoke(this, item);
            NewAuditEntry?.Invoke(this, item);
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(items);
            File.WriteAllText(JsonFile, json);
        }
    }
}
