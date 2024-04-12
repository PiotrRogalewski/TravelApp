namespace TravelApp.Repositories;

using TravelApp.Entities;

public interface IWriteReposoitory <in T> where T : class, IEntity
{
    void Add(T item);
    void Remove(T item);
    void Save();                       
}
