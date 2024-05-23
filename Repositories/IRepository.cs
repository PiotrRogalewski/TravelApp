namespace TravelApp.Repositories;

using TravelApp.Entities;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : class, IEntity
{
}