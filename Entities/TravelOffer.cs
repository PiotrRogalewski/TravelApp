using System.Text;

namespace TravelApp.Entities;

public class TravelOffer : TravelEntitiesBase
{
    public override string ToString()
    {
        StringBuilder sb = new(1024);
        sb.AppendLine($"{Country}\t\t(Offer Id: {Id})");
        sb.AppendLine($"\tCity: {City}\t\t{Region}");
        sb.AppendLine($"\tHotel: {Hotel}\t\tLowest Price: {LowestPrice}*");
        return sb.ToString();
    }
}
