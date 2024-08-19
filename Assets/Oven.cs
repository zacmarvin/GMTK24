using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oven : MonoBehaviour
{
    
    

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

            if(hoveredOver && Input.GetMouseButtonDown(0) && DistanceToPlayer < FirstPersonController.Instance.ReachDistance)
            {
                Transform child = FirstPersonController.Instance.transform.GetChild(0);

                DataObject dataObject = child.gameObject.GetComponent<DataObject>();
                
                if (dataObject == null || dataObject.thisFoodItemData.CurrentFoodType != FoodItemData.FoodType.EmptyDrink)
                {
                    return;
                }
                
                // Move child 0 to mouse position
                // Raycast to find position to place object
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
                {
                    if(FirstPersonController.Instance.transform.childCount == 0)
                    {
                        return;
                    }
                
                    placingObject = true;
                    child.parent = null;
                    hoveredOver = false;
                    placeRotation = dataObject.GetCurrentRotation();
                    placePosition = transform.position + new Vector3(-0.12f, -0.22f, 0.23f) + dataObject.GetCurrentOffset();
                    Debug.Log(placePosition);
                    _child = child;
                }
                
                
            }
            
            if(placingObject)
            {
                _child.position = Vector3.MoveTowards(_child.position, placePosition, 3f * Time.deltaTime);
                _child.rotation = placeRotation;
            
                if(Vector3.Distance(_child.position, placePosition) < 0.1f)
                {
                    placingObject = false;
                    EnableButtons();
                    _child.position = placePosition;
                    _child.parent = transform;
                    startTime = Time.time;
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
    
    IEnumerator Operating()
    {
        yield return new WaitForSeconds(operationDuration);
        operating = false;
        DisableButtons();
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
                
                if (dataObject != null  && dataObject.thisFoodItemData.CurrentFoodType == FoodItemData.FoodType.EmptyDrink)
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
