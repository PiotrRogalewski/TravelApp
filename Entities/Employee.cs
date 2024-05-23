namespace TravelApp.Entities;

public class Employee : EntityBase
{
    public override string ToString() => $"\n\t {Id} \t\t {FirstName} {LastName} {Suffix}";
}