namespace TravelApp.Entities
{
    public class EntityBase : IEntity
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
