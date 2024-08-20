using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DataObject : MonoBehaviour
{
    
    public FoodItemData thisFoodItemData;
    
    public CustomObject dataObject;

    public enum Scale
    {
        micro,
        small,
        medium,
        large,
        huge
    }
    
    public enum Temperature
    {
        cold,
        cool,
        warm,
        hot,
        burning
    }
    
    public Scale CurrentScale = Scale.medium;

    public Temperature CurrentTemperature = Temperature.cool;

    public Quaternion GetCurrentRotation()
    {
        return ((CustomObject)dataObject).defaultRotation;
    }


    private void Update()
    {
        if(transform.parent == FirstPersonController.Instance.transform)
        {
            // Get string of holding text
            
            FirstPersonController.Instance.HoldingText.enabled = true;
            FirstPersonController.Instance.HoldingText.text = "Holding " + GetFoodTypeName();
        }
    }

    string GetFoodTypeName()
    {
        string stringToReturn = "";
        switch (thisFoodItemData.CurrentFoodType)
        {
            case FoodItemData.FoodType.Burger:
                stringToReturn = GetFoodScaleName();
                stringToReturn += " " + GetFoodTemperatureName();
                stringToReturn += " Burger";
                break;
            case FoodItemData.FoodType.Hotdog:
                stringToReturn = GetFoodScaleName();
                stringToReturn += " " + GetFoodTemperatureName();
                stringToReturn += " Hotdog";
                break;
            case FoodItemData.FoodType.Taco:
                stringToReturn = GetFoodScaleName();
                stringToReturn += " " + GetFoodTemperatureName();
                stringToReturn += " Taco";
                break;
            case FoodItemData.FoodType.Popsicle:
                stringToReturn = GetFoodScaleName();
                stringToReturn += " " + GetFoodTemperatureName();
                stringToReturn += " Popsicle";
                break;
            case FoodItemData.FoodType.Drink:
                stringToReturn = GetFoodScaleName();
                stringToReturn += " " + GetFoodTemperatureName();
                stringToReturn += " " + GetDrinkTypeName();
                stringToReturn += " Drink";
                break;
            case FoodItemData.FoodType.EmptyDrink:
                stringToReturn = "Empty Drink";
                break;
            case FoodItemData.FoodType.Plate:
                if(transform.childCount > 0)
                {
                    stringToReturn = "Plate Of Food";
                }
                else
                {
                    stringToReturn = "Empty Plate";
                }
                break;
            default:
                return " Nothing";
        }

        return stringToReturn;
    }
    
    string GetDrinkTypeName()
    {
        string stringToReturn = "";
        switch (thisFoodItemData.CurrentDrinkType)
        {
            case FoodItemData.DrinkType.Red:
                stringToReturn += "Red";
                break;
            case FoodItemData.DrinkType.Green:
                stringToReturn += "Green";
                break;
            case FoodItemData.DrinkType.Blue:
                stringToReturn += "Blue";
                break;
            case FoodItemData.DrinkType.Yellow:
                stringToReturn += "Yellow";
                break;
            case FoodItemData.DrinkType.Brown:
                stringToReturn += "Brown";
                break;
        }
        return stringToReturn;
    }
    
    string GetFoodScaleName()
    {
        string stringToReturn = "";
        switch (CurrentScale)
        {
            case Scale.micro:
                stringToReturn += "Micro";
                break;
            case Scale.small:
                stringToReturn += "Small";
                break;
            case Scale.medium:
                stringToReturn += "Medium";
                break;
            case Scale.large:
                stringToReturn += "Large";
                break;
            case Scale.huge:
                stringToReturn += "Huge";
                break;
        }
        return stringToReturn;
    }
    
    string GetFoodTemperatureName()
    {
        string stringToReturn = "";
        switch (thisFoodItemData.CurrentTemperature)
        {
            case FoodItemData.Temperature.Freezing:
                stringToReturn += "Freezing";
                break;
            case FoodItemData.Temperature.Cold:
                stringToReturn += "Cool";
                break;
            case FoodItemData.Temperature.Warm:
                stringToReturn += "Warm";
                break;
            case FoodItemData.Temperature.Hot:
                stringToReturn += "Hot";
                break;
            case FoodItemData.Temperature.Burning:
                stringToReturn += "Burning";
                break;
            
        }
        return stringToReturn;
    }

    public Vector3 GetCurrentScale()
    {
        switch(CurrentScale)
        {
            case Scale.micro:
                return ((CustomObject)dataObject).microScale;
            case Scale.small:
                return ((CustomObject)dataObject).smallScale;
            case Scale.medium:
                return ((CustomObject)dataObject).mediumScale;
            case Scale.large:
                return ((CustomObject)dataObject).largeScale;
            case Scale.huge:
                return ((CustomObject)dataObject).hugeScale;
            default:
                return Vector3.zero;
        }
    }

    
    public Vector3 GetCurrentOffset()
    {
        switch(CurrentScale)
        {
            case Scale.micro:
                return ((CustomObject)dataObject).microOffsetPosition;
            case Scale.small:
                return ((CustomObject)dataObject).smallOffsetPosition;
            case Scale.medium:
                return ((CustomObject)dataObject).mediumOffsetPosition;
            case Scale.large:
                return ((CustomObject)dataObject).largeOffsetPosition;
            case Scale.huge:
                return ((CustomObject)dataObject).hugeOffsetPosition;
            default:
                return Vector3.zero;
        }
    }
    
    public Mesh GetCurrentMesh()
    {
        switch(CurrentTemperature)
        {
            case Temperature.cold:
                return ((CustomObject)dataObject).coldMesh;
            case Temperature.cool:
                return ((CustomObject)dataObject).coolMesh;
            case Temperature.warm:
                return ((CustomObject)dataObject).warmMesh;
            case Temperature.hot:
                return ((CustomObject)dataObject).hotMesh;
            case Temperature.burning:
                return ((CustomObject)dataObject).burningMesh;
            default:
                return null;
        }
    }
    
}
