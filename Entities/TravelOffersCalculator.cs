namespace TravelApp.Entities;

public class TravelOffersCalculator : TravelEntitiesBase
{
    string? Airline { get; set; } // 2-3 airline options
    string DepartureFromTheAirport { get; set; } // 4-10 airport options
    decimal FlightPrice { get; set; } // price depends on the airline and airport, always for both directions
    decimal PriceOfMealsPerDay { get; set; } // price depends on number of meals (5 options) - switch
    bool AllInclusive { get; set; } // yes/no
    decimal AllInclusivePerDay { get; set; } // stable unit

    float NumberOfPeople { get; set; }
    int TripDuration { get; set; }


    decimal PricePerDayForOnePerson { get; set; } // { (PricePerHotelDay + PriceOfMealsPerDay / AllInclusivePerDay (if AllInclusive == true)) }
    decimal TotalPrice { get; set; }// (NumberOfPeople x TripDuration x PricePerHotelDay) + (NumberOfPeople x TripDuration x PriceOfMealsPerDay / or AllInclusivePerDay) + FlightPrice


    public decimal GetPriceOfMealsPerDay() // price depends on number of meals (5 options) - switch
    {
        return PriceOfMealsPerDay;
    }

    public decimal GetFlightPrice() // price depends on the airline and airport
    {
        return FlightPrice;
    }

    public decimal GetTotalPrice() // all costs
    {
        return TotalPrice;
    }
}
