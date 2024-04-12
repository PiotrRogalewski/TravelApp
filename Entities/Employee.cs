namespace TravelApp.Entities;

public class Employee : EntityBase
{
    public override string ToString() => $"\n---[employee ID:]---[personal data:]----------------------------------------------------\n\n\t {Id} \t\t {FirstName} {LastName}";
}