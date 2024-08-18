using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonObjectComponent : MonoBehaviour
{
    [SerializeField]
    public UnityEvent OnButtonPressed;

    private bool hoveredOver = false;

    // Update is called once per frame
    void Update()
    {
        if (hoveredOver && Input.GetMouseButtonDown(0))
        {
            OnButtonPressed?.Invoke();
        }
    }

    private void OnMouseEnter()
    {
        if (!enabled)
        {
            return;
        }
        if (FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }

        hoveredOver = true;

        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.PickupCrosshairSprite;
    }

    private void OnMouseExit()
    {
        if (!enabled)
        {
            return;
        }
        if (FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }

        FirstPersonController.Instance.Crosshair.sprite = FirstPersonController.Instance.DefaultCrosshairSprite;
        hoveredOver = false;
    }
}