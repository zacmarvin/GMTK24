using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairLabel : MonoBehaviour
{
    [SerializeField] public Image Crosshair;
    
    [SerializeField] public Sprite PickupCrosshairSprite;
    
    [SerializeField] public Sprite DefaultCrosshairSprite;
    
    [SerializeField] public Sprite DropCrosshairSprite;
    
    [SerializeField] public Sprite BusyCrosshairSprite;
    
    [SerializeField] public Sprite ThrowSprite;
    
    [SerializeField] public TMP_Text CrosshairLabelTMPText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Crosshair.sprite == PickupCrosshairSprite)
        {
            CrosshairLabelTMPText.text = "Interact";
        }
        else if(Crosshair.sprite == DefaultCrosshairSprite)
        {
            CrosshairLabelTMPText.text = "";
        }
        else if(Crosshair.sprite == DropCrosshairSprite)
        {
            CrosshairLabelTMPText.text = "Drop";
        }
        else if(Crosshair.sprite == BusyCrosshairSprite)
        {
            CrosshairLabelTMPText.text = "Busy";
        }
        else if(Crosshair.sprite == ThrowSprite)
        {
            CrosshairLabelTMPText.text = "Throw";
        }
        else
        {
            CrosshairLabelTMPText.text = "";
        }
        
    }
}
