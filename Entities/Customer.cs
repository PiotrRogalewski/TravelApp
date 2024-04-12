namespace TravelApp.Entities;

public class Customer : EntityBase
{
    public override string ToString() => $"\n---[customer ID:]---[personal data:]----------------------------------------------------\n\n\t {Id} \t\t {FirstName} {LastName}";
}