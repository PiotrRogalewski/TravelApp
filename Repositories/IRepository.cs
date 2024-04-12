namespace TravelApp.Repositories;

using TravelApp.Entities;

public interface IRepository<T> : IReadRepository<T>, IWriteReposoitory<T>
    where T : class, IEntity
{
}