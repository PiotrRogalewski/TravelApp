using System.Text;

namespace TravelApp.Entities;

public class TravelOffer : TravelEntitiesBase
{
    public override string ToString()
    {
        StringBuilder sb = new(1024);
        sb.AppendLine($"\t{Country}\t\t(Offer Id: {Id})");
        sb.AppendLine($"\tRegion: {Region}, City: {City}");
        sb.AppendLine($"\tHotel: {Hotel}\t\tLowest Price: {LowestPrice}*");
        sb.AppendLine($"\n\t* - Price for one day of stay for one person including flight in both directions, does not include meals");
        return sb.ToString();
    }
}
