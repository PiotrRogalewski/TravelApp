namespace TravelApp.Entities
{
    public class PersonalEntitiesBase : IEntity
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
    }
}
