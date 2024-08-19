using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerComponent : MonoBehaviour
{
    public enum CustomerState
    {
        WalkingUp,
        WaitingInLine,
        UpToOrder,
        WaitingOnFood,
        Leaving
    }
    
    public FoodItemData.OrderSize RequiredOrderSize;
    
    [SerializeField]
    public List<FoodItemData> OrderItems = new List<FoodItemData>();

    private Vector3 cornerPosition = new Vector3(1, 1, 5.5f);
    
    private Vector3 linePosition1 = new Vector3(1, 1, 2f);
    
    private Vector3 linePosition2 = new Vector3(1, 1, 3f);
    
    private Vector3 linePosition3 = new Vector3(1, 1, 4f);
    
    private Vector3 waitPosition1 = new Vector3(-2.5f, 1, 2.5f);
    
    private Vector3 waitPosition2 = new Vector3(-2.5f, 1, 4f);
    
    private Vector3 waitPosition3 = new Vector3(-1.25f, 1, 4f);
    
    private Vector3 leavePosition = new Vector3(-13, 1, 5.5f);
    
    public CustomerState CurrentState = CustomerState.WalkingUp;
    
    public float moveSpeed = 1.5f;
    
    public int linePosition = 3;
    
    public int waitPosition = 3;
    
    public bool hoveredOver = false;

    private void OnEnable()
    {
        // Generate random order
        int orderCount = UnityEngine.Random.Range(1, 4);
        HashSet<FoodItemData.FoodType> usedFoodTypes = new HashSet<FoodItemData.FoodType>();

        for (int i = 0; i < orderCount; i++)
        {
            FoodItemData foodItemData = new FoodItemData();
            foodItemData.CurrentOrderSize = RequiredOrderSize;
    
            // Generate a unique FoodType that hasn't been used yet
            FoodItemData.FoodType newFoodType;
            do
            {
                newFoodType = (FoodItemData.FoodType)UnityEngine.Random.Range(0, 4);
            } while (usedFoodTypes.Contains(newFoodType));

            foodItemData.CurrentFoodType = newFoodType;
            usedFoodTypes.Add(newFoodType);

            if(foodItemData.CurrentFoodType == FoodItemData.FoodType.Drink)
            {
                foodItemData.CurrentDrinkType = (FoodItemData.DrinkType)UnityEngine.Random.Range(1, 6);
            }
    
            foodItemData.CurrentTemperature = (FoodItemData.Temperature)UnityEngine.Random.Range(1, 6);
    
            OrderItems.Add(foodItemData);
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case CustomerState.WalkingUp:
                transform.position = Vector3.MoveTowards(transform.position, cornerPosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, cornerPosition) < 0.1f)
                {
                    CheckCustomerPosition();
                    CurrentState = CustomerState.WaitingInLine;
                }
                break;
            case CustomerState.WaitingInLine:
                CheckCustomerPosition();
                if (linePosition == 3)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition3, moveSpeed * Time.deltaTime);
                }
                else if (linePosition == 2)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition2, moveSpeed * Time.deltaTime);
                }
                else if (linePosition == 1)
                {
                    transform.position = Vector3.MoveTowards(transform.position, linePosition1, moveSpeed * Time.deltaTime);
                    if (Vector3.Distance(transform.position, linePosition1) < 0.1f)
                    {
                        CurrentState = CustomerState.UpToOrder;
                    }
                }
                break;
            case CustomerState.WaitingOnFood:
                CheckWaitPosition();
                if (waitPosition == 3)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waitPosition3, moveSpeed * Time.deltaTime);
                }
                else if (waitPosition == 2)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waitPosition2, moveSpeed * Time.deltaTime);
                }
                else if (waitPosition == 1)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waitPosition1, moveSpeed * Time.deltaTime);
                }
                break;
            case CustomerState.Leaving:
                transform.position = Vector3.MoveTowards(transform.position, leavePosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, leavePosition) < 0.1f)
                {
                    Destroy(gameObject);
                }
                break;
        }
        
        if(hoveredOver && Input.GetMouseButtonDown(0) && CustomerManager.Instance.CustomersWaiting.Count < 3)
        {
            if (CurrentState == CustomerState.UpToOrder)
            {
                CustomerManager.Instance.WaitForFood(gameObject);
                CurrentState = CustomerState.WaitingOnFood;
            }
        }
        
        if(hoveredOver && Input.GetMouseButtonDown(1) && FirstPersonController.Instance.transform.childCount > 0)
        {
            if (CurrentState == CustomerState.WaitingOnFood)
            {
                GameObject child = FirstPersonController.Instance.transform.GetChild(0).gameObject;

                if (child.GetComponent<ThrowableComponent>())
                {
                    ThrowableComponent throwableComponent = child.GetComponent<ThrowableComponent>();
                    throwableComponent.Throw();
                    
                    CustomerManager.Instance.CustomersWaiting.Remove(gameObject);

                    Debug.Log("Order was:" + OrderItems);
                    Debug.Log("Order thrown was:" + child.GetComponent<DataObject>().thisFoodItemData);
                    // Loop through child children and log data object thisFoodItemData
                    foreach (Transform childTransform in child.transform)
                    {
                        Debug.Log(childTransform.GetComponent<DataObject>().thisFoodItemData);
                    }
                    StartCoroutine(DelayedLeave());  
                }
            }
        }
    }
    
    void CheckCustomerPosition()
    {
        // Get position in Customer Manager CustomersInLine
        int position = CustomerManager.Instance.CustomersInLine.IndexOf(gameObject);
        linePosition = position + 1;
    }
    
    void CheckWaitPosition()
    {
        // Get position in Customer Manager CustomersWaiting
        int position = CustomerManager.Instance.CustomersWaiting.IndexOf(gameObject);
        waitPosition = position + 1;
    }

    private void OnMouseEnter()
    {
        hoveredOver = true;
    }
    
    private void OnMouseExit()
    {
        hoveredOver = false;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
    }
    
    IEnumerator DelayedLeave()
    {
        yield return new WaitForSeconds(1);
        CurrentState = CustomerState.Leaving;
    }
}
