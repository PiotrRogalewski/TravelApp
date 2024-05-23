namespace TravelApp.Entities;

public class Customer : EntityBase
{
    public override string ToString() => $"\n\t {Id} \t\t {FirstName} {LastName} {Suffix}";
}