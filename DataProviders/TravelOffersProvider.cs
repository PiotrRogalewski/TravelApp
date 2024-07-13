using System.Text;
using TravelApp.DataProviders.Extensions;
using TravelApp.Entities;
using TravelApp.Repositories;

namespace TravelApp.DataProviders;

public class TravelOffersProvider : ITravelOffersProvider
{
    private readonly IRepository<TravelOffer> _travelOffersRepository;

    public TravelOffersProvider(IRepository<TravelOffer> travelOffersRepository)
    {
        _travelOffersRepository = travelOffersRepository;
    }

    public List<string> DisplayAllCountriesSeparately()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        var countries = travelOffers.Select(travelOffer => travelOffer.Country).Distinct().ToList();
        return countries;
    }

    public List<TravelOffer> GetSpecificColumns()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        var list = travelOffers.Select(travelOffer => new TravelOffer
        {
            Id = travelOffer.Id,
            Country = travelOffer.Country,
            City = travelOffer.City,
            LowestPrice = travelOffer.LowestPrice
        }).ToList();

        return list;
    }

    public string AnnonymousClass() // this is only example
    {
        var travelOffers = _travelOffersRepository.GetAll();
        var list = travelOffers.Select(travelOffer => new
        {
            Identifier = travelOffer.Id,
            Destination = travelOffer.Country,
            RestingPlace = travelOffer.City,
            BestPrice = travelOffer.LowestPrice
        });

        StringBuilder sb = new(2048);
        foreach (var travelOffer in list)
        {
            sb.AppendLine($"\tOffer ID: {travelOffer.Identifier}");
            sb.AppendLine($"\tDestination: {travelOffer.Destination}");
            sb.AppendLine($"\tResting place: {travelOffer.RestingPlace}");
            sb.AppendLine($"\tBest price for this offer: {travelOffer.BestPrice}");
        }
        return sb.ToString();
    }
    public List<TravelOffer> OrderByCountry()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.OrderBy(travelOffer => travelOffer.Country).ToList();
    }

    public List<TravelOffer> OrderByCountryDescending() // or Ascending = OrderByAscending
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.OrderByDescending(travelOffer => travelOffer.Country).ToList();
    }

    public List<TravelOffer> OrderByCountryAndCity()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .OrderBy(travelOffer => travelOffer.Country)
            .ThenBy(travelOffer => travelOffer.City)
            .ToList();
    }

    public List<TravelOffer> OrderByCountryAndPriceDescending()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .OrderByDescending(travelOffer => travelOffer.Country)
            .ThenByDescending(travelOffer => travelOffer.City)
            .ToList();
    }

    public List<TravelOffer> WhereStartsWith(string prefix)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.Where(travelOffer => travelOffer.Country.StartsWith(prefix)).ToList();
    }

    public List<TravelOffer> WhereStartsWithAndCostIsLowerOrEqualThan(string prefix, decimal cost)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.Where(travelOffer => travelOffer.Country.StartsWith(prefix) && travelOffer.LowestPrice <= cost).ToList();
    }

    public List<TravelOffer> WhereCountryIs(string country)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.ByCountry(country).ToList();
    }

    public TravelOffer FirstByCity(string city)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.First(x => x.City == city);
    }

    public TravelOffer? FirstOrDefaultByCity(string city)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.FirstOrDefault(x => x.City == city);
    }

    public TravelOffer FirstOrDefaultByCityWithDefault(string city)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .FirstOrDefault(
            x => x.City == city,
            new TravelOffer { Id = -1, City = "NOT FOUND" });
    }

    public TravelOffer LastByCity(string city)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.Last(x => x.City == city);
    }

    public TravelOffer SingleById(int id)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.Single(x => x.Id == id);
    }

    public TravelOffer? SingleOrDefaultById(int id)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.SingleOrDefault(x => x.Id == id);
    }

    public List<TravelOffer> TakeOffers(int howMany)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .OrderBy(x => x.Country)
            .Take(howMany)
            .ToList();
    }

    public List<TravelOffer> TakeOffers(Range range)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .OrderBy(x => x.Country)
            .Take(range) // np. range może być (2..7)
            .ToList();
    }

    public List<TravelOffer> TakeOffersWhileCountryStartsWith(string prefix)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
            .OrderBy(x => x.Country)
            .TakeWhile(x => x.Country.StartsWith(prefix))
            .ToList();
    }

    public List<TravelOffer> SkipOffer(int howMany)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
           .OrderBy(x => x.Country)
           .Skip(howMany)
           .ToList();
    }

    public List<TravelOffer> SkipOffersWhileCountryStartsWith(string prefix)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
          .OrderBy(x => x.Country)
          .SkipWhile(x => x.Country.StartsWith(prefix))
          .ToList();
    }

    public List<string> DistinctAllCountries()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
          .Select(x => x.Country)
          .Distinct()
          .OrderBy(x => x)
          .ToList();
    }

    public List<TravelOffer> DistinctByCountries()
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers
          .DistinctBy(x => x.Country)
          .OrderBy(x => x.Country)
          .ToList();
    }

    public List<TravelOffer[]> ChunkOffers(int size)
    {
        var travelOffers = _travelOffersRepository.GetAll();
        return travelOffers.Chunk(size).ToList();
    }
}
