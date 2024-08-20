using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShrinkGrowEffect : MonoBehaviour
{
    public Transform KnobArrow;

    public ButtonObjectComponent KnobButtonObjectComponent;
    
    public BoxCollider KnobBoxCollider;
    
    public enum Scale
    {
        small,
        medium,
        large
    }
    
    public Scale CurrentScale = Scale.medium;
    
    bool hoveredOver = false;
    
    bool placingObject = false;
    
    Vector3 placePosition;
    
    Quaternion placeRotation;
    
    Transform _child;

    private float operationDuration = 5f;
    
    private bool operating = false;
    
    private float startTime;
    
    private Quaternion startRotation;
    
    private Vector3 startPosition;
    
    private Vector3 startScale;
    
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            float DistanceToPlayer = Vector3.Distance(transform.position, Camera.main.transform.position);

            if (hoveredOver && Input.GetMouseButtonDown(0) && DistanceToPlayer < FirstPersonController.Instance.ReachDistance)
            {
                // Dont do anything if player is holding a plate
                if (FirstPersonController.Instance.transform.childCount > 0)
                {
                    Transform child = FirstPersonController.Instance.transform.GetChild(0);

                    DataObject dataObject = child.gameObject.GetComponent<DataObject>();

                    if (dataObject != null &&
                        dataObject.thisFoodItemData.CurrentFoodType == FoodItemData.FoodType.Plate)
                    {
                        return;
                    }
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
                
                    Transform child = FirstPersonController.Instance.transform.GetChild(0);
                    if(child.gameObject.GetComponent<DataObject>() != null)
                    {
                        DataObject dataObject = child.gameObject.GetComponent<DataObject>();
                        placingObject = true;
                        child.parent = null;
                        hoveredOver = false;
                        placeRotation = dataObject.GetCurrentRotation();
                        placePosition = transform.position + new Vector3(0, -0.1f, 0) + dataObject.GetCurrentOffset();
                        Debug.Log(placePosition);
                        _child = child;
                    }
                }
                
                
            }
            
            if(_child != null && placingObject)
            {
                _child.position = Vector3.MoveTowards(_child.position, placePosition, 3f * Time.deltaTime);
                _child.rotation = placeRotation;
            
                if(Vector3.Distance(_child.position, placePosition) < 0.1f)
                {
                    placingObject = false;
                    _child.position = placePosition;
                    _child.parent = transform;
                    startTime = Time.time;
                    KnobBoxCollider.enabled = true;
                    KnobButtonObjectComponent.enabled = true;
                    startScale = _child.localScale;
                    startPosition = _child.position;
                    startRotation = _child.rotation;
                }
            }
        }
        else
        {
            
            
            ScaledToSize();

            if (_child != null)
            {
                if (operating)
                {
                    // Spin and bounce _child
                    _child.Rotate(Vector3.up, 360f * Time.deltaTime, Space.World);
                    _child.position = new Vector3(startPosition.x, _child.position.y + Mathf.Sin(Time.time * 7) * 0.1f * Time.deltaTime, startPosition.z);
                }
                else
                {
                    // Set spin and rotation back
                    _child.rotation = startRotation;
                    _child.position = startPosition;
                
                }
            }

            if (hoveredOver && Input.GetMouseButtonDown(0) && !operating && FirstPersonController.Instance.transform.childCount == 0)
            {     
                Debug.Log("Child");
                Transform child = transform.GetChild(0);
                PickupEffect pickupEffect = child.gameObject.GetComponent<PickupEffect>();
                if (pickupEffect != null)
                {
                    pickupEffect.pickingUp = true;
                    pickupEffect.reachedPlayer = false;
                }

                hoveredOver = false;
                placingObject = false;
                _child = null;
                operating = false;
                KnobBoxCollider.enabled = false;
                KnobButtonObjectComponent.enabled = false;
                UpdateCrosshair();
            }
            
        }
        
    }

    bool ScaledToSize()
    {
        if(_child == null)
        {
            return false;
        }

        UpdateCrosshair();
        switch (CurrentScale)
        {
            case Scale.small:
                Vector3 intendedScale = _child.GetComponent<DataObject>().dataObject.smallScale;
                float smallPercenteDone = (Time.time - startTime) / operationDuration;
                _child.localScale = Vector3.Lerp(startScale, intendedScale, smallPercenteDone);
                if (Vector3.Distance(_child.localScale, intendedScale) < 0.01f)
                {
                    _child.GetComponent<DataObject>().thisFoodItemData.CurrentOrderSize = FoodItemData.OrderSize.Small;
                    _child.GetComponent<DataObject>().CurrentScale = DataObject.Scale.small;
                    return true;
                }
                else
                {
                    return false;
                }
            case Scale.medium:
                intendedScale = _child.GetComponent<DataObject>().dataObject.mediumScale;
                float mediumPercenteDone = (Time.time - startTime) / operationDuration;
                _child.localScale = Vector3.Lerp(startScale, intendedScale, mediumPercenteDone);
                if (Vector3.Distance(_child.localScale, intendedScale) < 0.01f)
                {
                    
                    _child.GetComponent<DataObject>().thisFoodItemData.CurrentOrderSize = FoodItemData.OrderSize.Medium;
                    _child.GetComponent<DataObject>().CurrentScale = DataObject.Scale.medium;
                    return true;
                }
                else
                {
                    return false;
                }
            case Scale.large:
                intendedScale = _child.GetComponent<DataObject>().dataObject.largeScale;
                
                float largePercenteDone = (Time.time - startTime) / operationDuration;
                _child.localScale = Vector3.Lerp(startScale, intendedScale, largePercenteDone);
                if (Vector3.Distance(_child.localScale, intendedScale) < 0.01f)
                {
                    _child.GetComponent<DataObject>().thisFoodItemData.CurrentOrderSize = FoodItemData.OrderSize.Large;
                    _child.GetComponent<DataObject>().CurrentScale = DataObject.Scale.large;
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return false;
        }
    }
    
    IEnumerator DelayStopOperation()
    {
        yield return new WaitForSeconds(operationDuration);
        operating = false;
        UpdateCrosshair();
    }
    
    public void ToggleKnob()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        startTime = Time.time;
        startScale = _child.localScale;
        operating = true;
        StopAllCoroutines();
        StartCoroutine(DelayStopOperation());
        UpdateCrosshair();
        switch (CurrentScale)
        {
            case Scale.small:
                KnobArrow.localEulerAngles = new Vector3(-90f, -37.172f, 90f);
                CurrentScale = Scale.medium;
                break;
            case Scale.medium:
                KnobArrow.localEulerAngles = new Vector3(-180f, -37.172f, 90f);
                CurrentScale = Scale.large;
                break;
            case Scale.large:
                KnobArrow.localEulerAngles = new Vector3(0f, -37.172f, 90f);
                CurrentScale = Scale.small;
                break;
        }
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
                
                if (dataObject != null && dataObject.thisFoodItemData.CurrentFoodType != FoodItemData.FoodType.Plate)
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
    
    private void OnMouseEnter()
    {
        hoveredOver = true;
        UpdateCrosshair();
    }
    
    private void OnMouseExit()
    {
        hoveredOver = false;
        UpdateCrosshair();
    }
}
