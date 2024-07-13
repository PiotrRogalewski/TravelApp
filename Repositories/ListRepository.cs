namespace TravelApp.Repositories;

using System.Text.Json;
using TravelApp.Entities;
using System.Collections.Generic;

public class ListRepository<T> : IRepository<T>
    where T : class, IEntity, new()
{
    //private readonly Action<T>? _itemAddedCallback;

    private static List<T> _items = new();
    private static string JsonFile;

    public ListRepository()
    {
        //_itemAddedCallback = itemAddedCallback;
    }

    public event EventHandler<T>? ItemAdded;
    public event EventHandler<T>? ItemRemoved;
    public event EventHandler<T>? ItemsCounted;
    public event EventHandler<T>? NewAuditEntry;

    public string GetJsonFileName(string entityName)
    {
        JsonFile = "List_of_" + entityName + ".Json";
        return JsonFile;
    }

    public IEnumerable<T> GetAll()
    {

        if (File.Exists(JsonFile))
        {
            try
            {
                string json = File.ReadAllText(JsonFile);
                _items = JsonSerializer.Deserialize<List<T>>(json)!;
            }
            catch (Exception exc)
            {
                Console.WriteLine("\n\t\t\tException catched!\n\t" + exc.Message + "\n\tIgnore this message if you see the list of items (or if this list should be empty).\n\n");
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n\tFile with the list of items does not exist yet \n\tor the Json static string field was not specified or returned correctly! \n\t(line 13 and 25-28 in {this.GetType().Name})\n");
            Console.ResetColor();
        }

        return (IEnumerable<T>)_items.ToList();
    }

    public T GetById(int id)
    {
        return _items.Single(item => item.Id == id);
    }

    public void Add(T item)
    {
        item.Id = _items.Count + 1;
        int newId = item.Id;

        for (newId = item.Id; newId < int.MaxValue; newId++)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\tChecking available ID numbers...");
            Console.ResetColor();

            item.Id = newId;

            if (_items.Any(x => x.Id == item.Id))
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\tId already taken. Looking for a new ID for this item\n");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\tSuccess! ID assigned");
                Console.ResetColor();
                item.Id = newId;
                break;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\t(Message from:\t{this.GetType().Name})");
        Console.ResetColor();
        _items.Add(item);
        item.ItemCounter = _items.Count;

        using (var writer = File.AppendText(JsonFile))
        {
            writer.WriteLine(item);
        }

        ItemAdded?.Invoke(this, item);
        ItemsCounted?.Invoke(this, item);
        NewAuditEntry?.Invoke(this, item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
        item.ItemCounter = _items.Count;
        ItemRemoved?.Invoke(this, item);
        ItemsCounted?.Invoke(this, item);
        NewAuditEntry?.Invoke(this, item);
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(_items);
        File.WriteAllText(JsonFile, json);
    }
}
