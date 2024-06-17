namespace TravelApp.Entities;

public class Employee : PersonalEntitiesBase
{
    public override string ToString() => $"\n\t {Id} \t\t {FirstName} {LastName} {Suffix}";
}