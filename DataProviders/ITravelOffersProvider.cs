using TravelApp.Entities;

namespace TravelApp.DataProviders;

public interface ITravelOffersProvider
{
    //filter offers by price (maxPrice)

    // Select
    List<string> DisplayAllCountriesSeparately();
    List<TravelOffer> GetSpecificColumns();
    string AnnonymousClass();

    // OrderBy
    List<TravelOffer> OrderByCountry();
    List<TravelOffer> OrderByCountryDescending();
    List<TravelOffer> OrderByCountryAndCity();
    List<TravelOffer> OrderByCountryAndPriceDescending();

    // Where
    List<TravelOffer> WhereStartsWith(string prefix);
    List<TravelOffer> WhereStartsWithAndCostIsLowerOrEqualThan(string prefix, decimal cost);
    List<TravelOffer> WhereCountryIs(string country);

    // First, Last, Single
    TravelOffer FirstByCity(string city);
    TravelOffer FirstOrDefaultByCity(string city);
    TravelOffer FirstOrDefaultByCityWithDefault(string city);
    TravelOffer LastByCity(string city);
    TravelOffer SingleById(int id);
    TravelOffer SingleOrDefaultById(int id);

    // Take
    List<TravelOffer> TakeOffers(int howMany);
    List<TravelOffer> TakeOffers(Range range);
    List<TravelOffer> TakeOffersWhileCountryStartsWith(string prefix);

    // Skip
    List<TravelOffer> SkipOffer(int howMany);
    List<TravelOffer> SkipOffersWhileCountryStartsWith(string prefix);

    // Distinct
    List<string> DistinctAllCountries();
    List<TravelOffer> DistinctByCountries();

    // Chunk
    List<TravelOffer[]> ChunkOffers(int size);
}
