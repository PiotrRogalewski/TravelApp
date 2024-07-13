//using TravelApp.Entities;
//using TravelApp.Repositories;

//namespace TravelApp.DataProviders;

//public class TravelOffersProviderBasic : ITravelOffersProvider
//{
//    private readonly IRepository<TravelOffer> _travelOffersRepository;

//    public TravelOffersProviderBasic(IRepository<TravelOffer> travelOffersRepository)
//    {
//        _travelOffersRepository = travelOffersRepository;
//    }

//    public List<string> DisplayAllCountriesSeparately()
//    {
//        var offers = _travelOffersRepository.GetAll();
//        List<string> list = new();

//        foreach (var offer in offers)
//        {
//            if (!list.Contains(offer.Country))
//            {
//                list.Add(offer.Country);
//            }
//        }

//        return list;
//    }

//    public List<TravelOffer> FilterOffersByCountry(string country)
//    {
//        var offers = _travelOffersRepository.GetAll();
//        List<TravelOffer> list = new();

//        foreach (var offer in offers)
//        {
//            if (country == offer.Country)
//            {
//                list.Add(offer);
//            }
//        }

//        return list;
//    }

//    public List<TravelOffer> FilterOffersByPrice(decimal maxPrice)
//    {
//        var offers = _travelOffersRepository.GetAll();
//        var list = new List<TravelOffer>();

//        foreach (TravelOffer offer in offers)
//        {
//            if (offer.LowestPrice <= maxPrice)
//            {
//                list.Add(offer);
//            }
//        }
//        return list;
//    }
//}