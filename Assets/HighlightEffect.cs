using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
    
    public Material HighlightMaterial;
    
    private Material _defaultMaterial;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _defaultMaterial = GetComponent<Renderer>().material;
    }
    
    private void OnMouseEnter()
    {
        if (!enabled)
        {
            return;
        }
        if(FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }
        
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.PickupCrosshairSprite;
        GetComponent<Renderer>().material = HighlightMaterial;
    }
    
    private void OnMouseExit()
    {
        if (!enabled)
        {
            return;
        }
        if(FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }
        
        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
        GetComponent<Renderer>().material = _defaultMaterial;
    }
}
