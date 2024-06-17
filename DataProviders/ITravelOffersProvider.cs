using TravelApp.Entities;

namespace TravelApp.DataProviders;

public interface ITravelOffersProvider
{
    List<TravelOffer> FilterOffersByPrice(decimal price);
    List<TravelOffer> FilterOffersByCountry(string country);
    List<string> DisplayAllCountriesSeparately();
}
