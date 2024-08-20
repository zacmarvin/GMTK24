using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public int CustomerNumber = 0;

    public Canvas FloatingCustomerNumberCanvas;
    
    public TMP_Text FloatingCustomerNumberText;
    
    [SerializeField]
    public List<FoodItemData> OrderItems = new List<FoodItemData>();
    
    public Vector3 minimumScale = new Vector3(0.15f, 0.15f, 0.15f);
    
    public Vector3 maximumScale = new Vector3(0.85f, 0.85f, 0.85f);
    
    private MeshFilter meshFilter;

    private Vector3 cornerPosition = new Vector3(1, 0.1f, 5.5f);
    private Vector3 linePosition1 = new Vector3(1, 0.1f, 2f);
    private Vector3 linePosition2 = new Vector3(1, 0.1f, 3f);
    private Vector3 linePosition3 = new Vector3(1, 0.1f, 4f);
    private Vector3 waitPosition1 = new Vector3(-2.5f, 0.1f, 2.5f);
    private Vector3 waitPosition2 = new Vector3(-2.5f, 0.1f, 4f);
    private Vector3 waitPosition3 = new Vector3(-1.25f, 0.1f, 4f);
    private Vector3 leavePosition = new Vector3(-13, 0.1f, 5.5f);

    public CustomerState CurrentState = CustomerState.WalkingUp;

    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2.0f; // Speed of rotation

    public int linePosition = 3;
    public int waitPosition = 3;

    public bool hoveredOver = false;

    private Animator _animator;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();

        AssignRandomScale();
        
        // Loop through all materials in all skinned mesh renderers and assign random base color
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            foreach (Material material in skinnedMeshRenderer.materials)
            {
                AssignRandomBaseColor(material);
            }
        }

        
        // Generate random order
        int orderCount = UnityEngine.Random.Range(1, 4);
        HashSet<FoodItemData.FoodType> usedFoodTypes = new HashSet<FoodItemData.FoodType>();

        for (int i = 0; i < orderCount; i++)
        {
            FoodItemData foodItemData = new FoodItemData();
            // Generate a unique FoodType that hasn't been used yet
            FoodItemData.FoodType newFoodType;
            do
            {
                newFoodType = (FoodItemData.FoodType)UnityEngine.Random.Range(0, 5);
            } while (usedFoodTypes.Contains(newFoodType));

            foodItemData.CurrentFoodType = newFoodType;
            usedFoodTypes.Add(newFoodType);

            if (foodItemData.CurrentFoodType == FoodItemData.FoodType.Drink)
            {
                foodItemData.CurrentDrinkType = (FoodItemData.DrinkType)UnityEngine.Random.Range(1, 6);
            }

            foodItemData.CurrentTemperature = (FoodItemData.Temperature)UnityEngine.Random.Range(1, 6);
            foodItemData.CurrentOrderSize = (FoodItemData.OrderSize)UnityEngine.Random.Range(1, 4);

            OrderItems.Add(foodItemData);
        }
    }

    private void AssignRandomScale()
    {
        float randomScale = UnityEngine.Random.Range(minimumScale.x, maximumScale.x);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        
    }

    private void AssignRandomBaseColor(Material material)
    {
        // Check if the material has a "_Color" property before trying to access it
        if (material.HasProperty("_Color"))
        {
            // Change hue of material base color to random value
            Color.RGBToHSV(material.color, out float h, out float s, out float v);
            h = UnityEngine.Random.Range(0.0f, 1.0f);
            material.color = Color.HSVToRGB(h, s, v);
        }
        else
        {
            Debug.LogWarning($"Material '{material.name}' doesn't have a '_Color' property.");
        }

        // Check if the material has a "_ColorDim" property before trying to access it
        if (material.HasProperty("_ColorDim"))
        {
            // Get the _ColorDim property color
            Color colorDim = material.GetColor("_ColorDim");

            // Change hue of _ColorDim to the same random value
            Color.RGBToHSV(colorDim, out float hDim, out float sDim, out float vDim);
            hDim = UnityEngine.Random.Range(0.0f, 1.0f);
            Color newColorDim = Color.HSVToRGB(hDim, sDim, vDim);

            // Apply the new _ColorDim color to the material
            material.SetColor("_ColorDim", newColorDim);
        }
        else
        {
            Debug.LogWarning($"Material '{material.name}' doesn't have a '_ColorDim' property.");
        }
    }

    
    void Update()
    {
        switch (CurrentState)
        {
            case CustomerState.WalkingUp:
                MoveAndRotateTowards(cornerPosition);
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
                    MoveAndRotateTowards(linePosition3);
                }
                else if (linePosition == 2)
                {
                    MoveAndRotateTowards(linePosition2);
                }
                else if (linePosition == 1)
                {
                    MoveAndRotateTowards(linePosition1);
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
                    MoveAndRotateTowards(waitPosition3);
                }
                else if (waitPosition == 2)
                {
                    MoveAndRotateTowards(waitPosition2);
                }
                else if (waitPosition == 1)
                {
                    MoveAndRotateTowards(waitPosition1);
                }
                break;
            case CustomerState.UpToOrder:
                SmoothlyFacePlayer();
                _animator.SetTrigger("Idle");
                break;
            case CustomerState.Leaving:
                MoveAndRotateTowards(leavePosition);
                if (Vector3.Distance(transform.position, leavePosition) < 0.1f)
                {
                    Destroy(gameObject);
                }
                break;
        }

        if (hoveredOver && Input.GetMouseButtonDown(0) && CustomerManager.Instance.CustomersWaiting.Count < 3)
        {
            if (CurrentState == CustomerState.UpToOrder)
            {
                CustomerManager.Instance.WaitForFood(gameObject);
                CurrentState = CustomerState.WaitingOnFood;
            }
        }

        if (hoveredOver && Input.GetMouseButtonDown(1) && FirstPersonController.Instance.transform.childCount > 0)
        {
            if (CurrentState == CustomerState.WaitingOnFood)
            {
                GameObject child = FirstPersonController.Instance.transform.GetChild(0).gameObject;

                if (child.GetComponent<ThrowableComponent>())
                {
                    ThrowableComponent throwableComponent = child.GetComponent<ThrowableComponent>();
                    throwableComponent.Throw();

                    CustomerManager.Instance.UpdateAllTickets();

                    CheckOrderFulfillment(child);
                    StartCoroutine(DelayedLeave());
                }
            }
        }
    }
    
    private void CheckOrderFulfillment(GameObject child)
    {
        float totalFulfillment = 0f;
        int orderCount = OrderItems.Count;
        
        foreach (var orderItem in OrderItems)
        {
            float highestFulfillment = 0f;

            // Loop through the child objects to find the best match
            foreach (Transform childTransform in child.transform)
            {
                var thrownItem = childTransform.GetComponent<DataObject>().thisFoodItemData;

                if (thrownItem != null)
                {
                    // Calculate the fulfillment percentage for this child item
                    float fulfillmentPercentage = CalculateOrderFulfillment(orderItem, thrownItem);

                    // If this fulfillment percentage is higher, update the highestFulfillment
                    if (fulfillmentPercentage > highestFulfillment)
                    {
                        highestFulfillment = fulfillmentPercentage;
                    }
                }
            }

            // Accumulate the highest fulfillment percentage for this order item
            totalFulfillment += highestFulfillment;

            Debug.Log($"Order item: {orderItem.CurrentFoodType}, Best Fulfillment: {highestFulfillment}%");
        }
        
        // Calculate the average fulfillment percentage
        float averageFulfillment = totalFulfillment / orderCount;

        // 100% fulfillment add 10 to customer satisfaction
        // 50% does nothing
        // 0% subtract 10 from customer satisfaction
        float satisfactionChange = 30f * (averageFulfillment / 100f) - 10f;
        Debug.Log("Satisfaction Change: " + satisfactionChange);
        
        CustomerManager.Instance.customerSatisfaction += Mathf.RoundToInt(satisfactionChange);
        CustomerManager.Instance.UpdateCustomerSatisfactionUI();
    }

    private float CalculateOrderFulfillment(FoodItemData orderItem, FoodItemData thrownItem)
    {
        int totalAttributes = 3; // FoodType, Temperature, and OrderSize are always compared
        int matchedAttributes = 0;

        // Compare FoodType
        if (orderItem.CurrentFoodType == thrownItem.CurrentFoodType || thrownItem.CurrentFoodType == FoodItemData.FoodType.Drink && thrownItem.CurrentFoodType == FoodItemData.FoodType.EmptyDrink)
        {
            matchedAttributes++;
        }
        else
        {
            return 0;
        }

        // Compare Temperature
        if (orderItem.CurrentTemperature == thrownItem.CurrentTemperature || orderItem.CurrentTemperature == FoodItemData.Temperature.None)
        {
            matchedAttributes++;
        }

        // Compare OrderSize
        if (orderItem.CurrentOrderSize == thrownItem.CurrentOrderSize || orderItem.CurrentOrderSize == FoodItemData.OrderSize.None)
        {
            matchedAttributes++;
        }

        // If it's a drink, compare DrinkType
        if (orderItem.CurrentFoodType == FoodItemData.FoodType.Drink || orderItem.CurrentFoodType == FoodItemData.FoodType.EmptyDrink)
        {
            totalAttributes++;
            if (orderItem.CurrentDrinkType == thrownItem.CurrentDrinkType || orderItem.CurrentDrinkType == FoodItemData.DrinkType.None)
            {
                matchedAttributes++;
            }
        }

        // Calculate percentage of fulfilled attributes
        return (float)matchedAttributes / totalAttributes * 100f;
    }

    
    private void MoveAndRotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _animator.SetTrigger("Idle");
            SmoothlyFacePlayer();

        }
        else
        {
            _animator.SetTrigger("Move");
        }
    }

    private void SmoothlyFacePlayer()
    {
        if(FirstPersonController.Instance.transform == null)
        {
            return;
        }
        
        Vector3 playerPosition = FirstPersonController.Instance.transform.position;
        playerPosition.y = transform.position.y; // Keep rotation on the horizontal plane

        // Calculate direction and target rotation
        Vector3 direction = (playerPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
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

        if (CurrentState == CustomerState.UpToOrder)
        {
            FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.PickupCrosshairSprite;
        }
        else if (CurrentState == CustomerState.WaitingOnFood)
        {
            if (FirstPersonController.Instance.transform.childCount > 0)
            {
                Transform child = FirstPersonController.Instance.transform.GetChild(0);
                if (child.GetComponent<DataObject>())
                {
                    DataObject dataObject = child.GetComponent<DataObject>();
                    if (dataObject.thisFoodItemData.CurrentFoodType == FoodItemData.FoodType.Plate)
                    {
                        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.ThrowSprite;
                    }
                }
            }
        }
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
        CustomerManager.Instance.CustomersWaiting.Remove(gameObject);
        CustomerManager.Instance.UpdateAllTickets();
    }
}
