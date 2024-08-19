using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShrinkGrowEffect : MonoBehaviour
{
    bool hoveredOver = false;
    
    bool placingObject = false;
    
    Vector3 placePosition;
    
    Quaternion placeRotation;
    
    Transform _child;

    private float operationDuration = 3f;
    
    private bool operating = false;
    
    private float startTime;
    
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            if(hoveredOver && Input.GetMouseButtonDown(0))
            {
                // Move child 0 to mouse position
                // Raycast to find position to place object
                RaycastHit hit;
                if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f))
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
                        placePosition = transform.position + dataObject.GetCurrentOffset();
                        Debug.Log(placePosition);
                        _child = child;
                    }
                }
                
                
            }
            
            if(placingObject)
            {
                _child.position = Vector3.MoveTowards(_child.position, placePosition, 3f * Time.deltaTime);
                _child.rotation = placeRotation;
            
                if(Vector3.Distance(_child.position, placePosition) < 0.1f)
                {
                    placingObject = false;
                    _child.position = placePosition;
                    _child.parent = transform;
                    startTime = Time.time;
                    operating = true;
                }
            }
        }
        else
        {
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
            }
        }
        
    }
    
    private void OnMouseEnter()
    {
        hoveredOver = true;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.ScrollPlusClickCrosshairSprite;
        FirstPersonController.Instance.Crosshair.gameObject.GetComponent<RectTransform>().sizeDelta =
            new Vector2(96, 96);
    }
    
    private void OnMouseExit()
    {
        hoveredOver = false;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
        FirstPersonController.Instance.Crosshair.gameObject.GetComponent<RectTransform>().sizeDelta =
            new Vector2(48, 48);
    }
}
