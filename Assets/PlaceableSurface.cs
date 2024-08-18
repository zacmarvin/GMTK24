using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    bool hoveredOver = false;
    
    bool placingObject = false;
    
    Vector3 placePosition;
    
    Quaternion placeRotation;
    
    Transform _child;
    
    public bool isPlate = false;
    
    // Update is called once per frame
    void Update()
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
                    placePosition = hit.point + dataObject.GetCurrentOffset();
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
                if (_child.GetComponent<PickupEffect>())
                {
                    PickupEffect pickupEffect = _child.GetComponent<PickupEffect>();
                    pickupEffect.ResetValues();
                }
                if(isPlate)
                {
                    _child.parent = transform;
                }
            }
        }
        
    }
    
    private void OnMouseEnter()
    {
        if(FirstPersonController.Instance.transform.childCount == 0)
        {
            return;
        }
        hoveredOver = true;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DropCrosshairSprite;
    }

    private void OnMouseDrag()
    {
        if(FirstPersonController.Instance.transform.childCount == 0)
        {
            return;
        }
        hoveredOver = true;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DropCrosshairSprite;
    }

    private void OnMouseExit()
    {
        
        if(FirstPersonController.Instance.transform.childCount == 0)
        {
            return;
        }
        hoveredOver = false;
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
    }
}
