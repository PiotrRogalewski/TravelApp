﻿namespace TravelApp.Entities;

public class Customer : PersonalEntitiesBase
{
    public override string ToString() => $"\n\t {Id} \t\t {FirstName} {LastName} {Suffix}";
}