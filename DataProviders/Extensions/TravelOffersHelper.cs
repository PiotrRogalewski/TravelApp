using TravelApp.Entities;

namespace TravelApp.DataProviders.Extensions
{
    public static class TravelOffersHelper
    {
        public static IEnumerable<TravelOffer> ByCountry(this IEnumerable<TravelOffer> query, string country)
        { 
        return query.Where(x => x.City == country);
        }
    }
}
