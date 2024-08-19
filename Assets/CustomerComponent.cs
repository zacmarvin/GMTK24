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
                newFoodType = (FoodItemData.FoodType)UnityEngine.Random.Range(0, 4);
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
        // Change hue of material base color to random value
        Color.RGBToHSV(material.color, out float h, out float s, out float v);
        h = UnityEngine.Random.Range(0.0f, 1.0f);
        material.color = Color.HSVToRGB(h, s, v);

        // if material has _colorDim property, change hue of that color to the same random value
        if (material.HasProperty("_ColorDim"))
        {
            // Get the _ColorDim property color
            Color colorDim = material.GetColor("_ColorDim");
    
            // Change hue of _ColorDim to the same random value
            Color.RGBToHSV(colorDim, out float hDim, out float sDim, out float vDim);
            hDim = h; // Use the same hue as the base color
            Color newColorDim = Color.HSVToRGB(hDim, sDim, vDim);

            // Apply the new _ColorDim color to the material
            material.SetColor("_ColorDim", newColorDim);
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
