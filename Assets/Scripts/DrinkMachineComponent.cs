using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkMachineComponent : MonoBehaviour
{
    
    
    public ParticleSystem RedDrinkParticles;
    public ParticleSystem GreenDrinkParticles;
    public ParticleSystem BlueDrinkParticles;
    public ParticleSystem YellowDrinkParticles;
    public ParticleSystem BrownDrinkParticles;

    public ButtonObjectComponent RedDrinkButton;
    public ButtonObjectComponent GreenDrinkButton;
    public ButtonObjectComponent BlueDrinkButton;
    public ButtonObjectComponent YellowDrinkButton;
    public ButtonObjectComponent BrownDrinkButton;
    
    public HighlightEffect RedDrinkHighlight;
    public HighlightEffect GreenDrinkHighlight;
    public HighlightEffect BlueDrinkHighlight;
    public HighlightEffect YellowDrinkHighlight;
    public HighlightEffect BrownDrinkHighlight;
    
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
                    EnableDrinkButtons();
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
                DisableDrinkButtons();
                UpdateCrosshair();
            }
        }
        
    }
    
    void EnableDrinkButtons()
    {
        RedDrinkButton.enabled = true;
        GreenDrinkButton.enabled = true;
        BlueDrinkButton.enabled = true;
        YellowDrinkButton.enabled = true;
        BrownDrinkButton.enabled = true;
        RedDrinkHighlight.enabled = true;
        GreenDrinkHighlight.enabled = true;
        BlueDrinkHighlight.enabled = true;
        YellowDrinkHighlight.enabled = true;
        BrownDrinkHighlight.enabled = true;
        
        
    }
    
    void DisableDrinkButtons()
    {
        RedDrinkButton.enabled = false;
        GreenDrinkButton.enabled = false;
        BlueDrinkButton.enabled = false;
        YellowDrinkButton.enabled = false;
        BrownDrinkButton.enabled = false;
        RedDrinkHighlight.enabled = false;
        GreenDrinkHighlight.enabled = false;
        BlueDrinkHighlight.enabled = false;
        YellowDrinkHighlight.enabled = false;
        BrownDrinkHighlight.enabled = false;
    }
    
        

    public void SetDrinkColor(string color)
    {
        RedDrinkParticles.Stop();
        GreenDrinkParticles.Stop();
        BlueDrinkParticles.Stop();
        YellowDrinkParticles.Stop();
        BrownDrinkParticles.Stop();
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        
        DataObject dataObject = _child.gameObject.GetComponent<DataObject>();
        
        dataObject.thisFoodItemData.CurrentFoodType = FoodItemData.FoodType.Drink;

        operating = true;
        StartCoroutine(Operating());

        switch(color)
        {
            case "Red":
                RedDrinkParticles.Play();
                dataObject.thisFoodItemData.CurrentDrinkType = FoodItemData.DrinkType.Red;
                break;
            case "Green":
                GreenDrinkParticles.Play();
                dataObject.thisFoodItemData.CurrentDrinkType = FoodItemData.DrinkType.Green;
                
                break;
            case "Blue":
                BlueDrinkParticles.Play();
                dataObject.thisFoodItemData.CurrentDrinkType = FoodItemData.DrinkType.Blue;
                break;
            case "Yellow":
                YellowDrinkParticles.Play();
                dataObject.thisFoodItemData.CurrentDrinkType = FoodItemData.DrinkType.Yellow;
                break;
            case "Brown":
                BrownDrinkParticles.Play();
                dataObject.thisFoodItemData.CurrentDrinkType = FoodItemData.DrinkType.Brown;
                break;
            default:
                break;
        }
        
        
    }
    
    IEnumerator Operating()
    {
        yield return new WaitForSeconds(operationDuration);
        operating = false;
        DisableDrinkButtons();
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
