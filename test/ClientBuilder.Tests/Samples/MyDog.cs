using System;
using System.Collections.Generic;

namespace ClientBuilder.Tests.Samples;

public class MyDog : MyBaseDog<FoodPreference, DateTime>
{
    public AnimalGang<Dog> Friends { get; set; }
}