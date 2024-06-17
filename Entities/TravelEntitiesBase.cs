namespace TravelApp.Entities;

public class TravelEntitiesBase : IEntity
{
    public int Id { get; set; }
    public string Country { get; set; }
    public string? Region { get; set; }
    public string City { get; set; }
    public string Hotel { get; set; }
    public decimal PricePerHotelDay { get; set; } // price for hotel only (without meals)
    public decimal LowestPrice { get; set; } // price for one day of stay for one person including flight in both directions, does not include meals
}
