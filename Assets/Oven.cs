using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Oven : MonoBehaviour
{
    [SerializeField]
    public UnityEvent CloseDoor;

    public enum OvenState
    {
        Frozen,
        Cold,
        Warm,
        Hot,
        Burnt
    }
    [SerializeField]
    ParticleSystem particleSystem;
    
    public OvenState ovenState = OvenState.Warm;

    public DoorHinge doorHinge;
    
    public ButtonObjectComponent FreezeButton;
    public ButtonObjectComponent CoolButton;
    public ButtonObjectComponent WarmButton;
    public ButtonObjectComponent HotButton;
    public ButtonObjectComponent BurnButton;
    
    public HighlightEffect FreezeButtonHighlight;
    public HighlightEffect CoolButtonHighlight;
    public HighlightEffect WarmButtonHighlight;
    public HighlightEffect HotButtonHighlight;
    public HighlightEffect BurnHighlight;
    
    bool hoveredOver = false;
    
    bool placingObject = false;
    
    Vector3 placePosition;
    
    Quaternion placeRotation;
    
    Transform _child;

    private float operationDuration = 7f;
    
    private bool operating = false;
    
    private float startTime;
    
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            float DistanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);

            if(hoveredOver && Input.GetMouseButtonDown(0) && DistanceToPlayer < FirstPersonController.Instance.ReachDistance && FirstPersonController.Instance.transform.childCount > 0)
            {
                Transform child = FirstPersonController.Instance.transform.GetChild(0);

                DataObject dataObject = child.gameObject.GetComponent<DataObject>();
                
                if (dataObject == null || dataObject.thisFoodItemData.CurrentFoodType == FoodItemData.FoodType.EmptyDrink)
                {
                    return;
                }
                
                placingObject = true;
                child.parent = null;
                hoveredOver = false;
                placeRotation = dataObject.GetCurrentRotation();
                placePosition = transform.position  + dataObject.GetCurrentOffset();
                Debug.Log(placePosition);
                _child = child;
                
                
            }
            
            if(placingObject)
            {
                _child.position = Vector3.MoveTowards(_child.position, placePosition, 3f * Time.deltaTime);
                _child.rotation = placeRotation;
            
                if(Vector3.Distance(_child.position, placePosition) < 0.1f)
                {
                    placingObject = false;
                    CloseDoor?.Invoke();
                    EnableButtons();
                    _child.position = placePosition;
                    _child.parent = transform;
                    startTime = Time.time;
                    PickupEffect pickupEffect = _child.gameObject.GetComponent<PickupEffect>();
                    if (pickupEffect != null)
                    {
                        pickupEffect.enabled = false;
                    }
                }
            }
        }
        else
        {
            // Check if child has pickup  effect and if that pickup effect is hovered over
            PickupEffect childPickupEffect = transform.GetChild(0).gameObject.GetComponent<PickupEffect>();
            
            if(childPickupEffect != null && childPickupEffect.hoveredOver)
            {
                hoveredOver = childPickupEffect.hoveredOver;
            }
            
            
            if (hoveredOver && Input.GetMouseButtonDown(0) && !operating)
            {     
                Debug.Log("Child");
                Transform child = transform.GetChild(0);
                PickupEffect pickupEffect = child.gameObject.GetComponent<PickupEffect>();
                if(pickupEffect != null)
                {
                    pickupEffect.pickingUp = true;
                    pickupEffect.reachedPlayer = false;
                }
                
                hoveredOver = false;
                placingObject = false;
                child = null;
            }
            
            if(operating && Time.time - startTime >= operationDuration)
            {
                operating = false;

                DisableButtons();
                UpdateCrosshair();
            }
        }
        
    }
    
    void EnableButtons()
    {
        FreezeButton.enabled = true;
        CoolButton.enabled = true;
        WarmButton.enabled = true;
        HotButton.enabled = true;
        BurnButton.enabled = true;
        FreezeButtonHighlight.enabled = true;
        CoolButtonHighlight.enabled = true;
        WarmButtonHighlight.enabled = true;
        HotButtonHighlight.enabled = true;
        BurnHighlight.enabled = true;
    }
    
    void DisableButtons()
    {
        FreezeButton.enabled = false;
        CoolButton.enabled = false;
        WarmButton.enabled = false;
        HotButton.enabled = false;
        BurnButton.enabled = false;
        FreezeButtonHighlight.enabled = false;
        CoolButtonHighlight.enabled = false;
        WarmButtonHighlight.enabled = false;
        HotButtonHighlight.enabled = false;
        BurnHighlight.enabled = false;
    }
    
    public void SetOvenState(int state)
    {
        if(doorHinge.doorState != DoorHinge.DoorState.Closed)
        {
            doorHinge.ToggleDoor();
        }
        
        ovenState = (OvenState)state;

        operating = true;
        StartCoroutine(Operating());
    }
    
    IEnumerator Operating()
    {
        particleSystem.Play();
        yield return new WaitForSeconds(operationDuration);
        operating = false;
        DisableButtons();
        PickupEffect pickupEffect = _child.gameObject.GetComponent<PickupEffect>();
        if (pickupEffect != null)
        {
            pickupEffect.enabled = true;
            pickupEffect.hoveredOver = false;
            pickupEffect.pickingUp = false;
        }
        particleSystem.Stop();
        Transform child = transform.GetChild(0);
        DataObject dataObject = child.gameObject.GetComponent<DataObject>();
        if(dataObject != null)
        {
            if(ovenState == OvenState.Frozen)
            {
                dataObject.thisFoodItemData.CurrentTemperature = FoodItemData.Temperature.Freezing;
            }
            else if(ovenState == OvenState.Cold)
            {
                dataObject.thisFoodItemData.CurrentTemperature = FoodItemData.Temperature.Cold;
            }
            else if(ovenState == OvenState.Warm)
            {
                dataObject.thisFoodItemData.CurrentTemperature = FoodItemData.Temperature.Warm;
            }
            else if(ovenState == OvenState.Hot)
            {
                dataObject.thisFoodItemData.CurrentTemperature = FoodItemData.Temperature.Hot;
            }
            else if(ovenState == OvenState.Burnt)
            {
                dataObject.thisFoodItemData.CurrentTemperature = FoodItemData.Temperature.Burning;
            }
        }
    }
    
    private void OnMouseEnter()
    {
        hoveredOver = true;
        UpdateCrosshair();
    }

    public void UpdateCrosshair()
    {
        if (operating)
        {
            FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.BusyCrosshairSprite;
        }
        else
        {
            if(FirstPersonController.Instance.transform.childCount > 0)
            {
                Transform child = FirstPersonController.Instance.transform.GetChild(0);

                DataObject dataObject = child.gameObject.GetComponent<DataObject>();
                
                if (dataObject != null  && dataObject.thisFoodItemData.CurrentFoodType != FoodItemData.FoodType.EmptyDrink)
                {
                    FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DropCrosshairSprite;
                }
            }
            else if (transform.childCount > 0)
            {
                FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.PickupCrosshairSprite;
            }
        }
    }

    private void OnMouseDrag()
    {
        hoveredOver = true;

        UpdateCrosshair();
    }

    private void OnMouseExit()
    {
        hoveredOver = false;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
    }
}
