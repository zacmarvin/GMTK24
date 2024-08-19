using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class FoodItemData
{
    public FoodItemData()
    {
        CurrentFoodType = FoodType.Plate;
        CurrentTemperature = Temperature.None;
        CurrentOrderSize = OrderSize.None;
        CurrentDrinkType = DrinkType.None;
    }

    public enum FoodType
    {
        Burger,
        Taco,
        Hotdog,
        Drink,
        Popsicle,
        // Non orderable items
        EmptyDrink,
        Plate
    }
    
    public FoodType CurrentFoodType;

    public enum Temperature
    {
        None,
        Freezing,
        Cold,
        Warm,
        Hot,
        Burning
    }
    
    public Temperature CurrentTemperature = Temperature.None;

    public enum OrderSize
    {
        None,
        Small,
        Medium,
        Large
    }
    
    [FormerlySerializedAs("CurrentSize")] 
    public OrderSize CurrentOrderSize = OrderSize.None;
    
    public enum DrinkType
    {
        None,
        Red,
        Green,
        Blue,
        Yellow,
        Brown
    }
    
    public DrinkType CurrentDrinkType = DrinkType.None;
}