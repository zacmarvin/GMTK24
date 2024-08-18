using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGrowComponent : MonoBehaviour
{
    private float _shrinkSpeed = 1f;
    
    private float _growSpeed = 1f;

    public void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PlayerController.Instance.CurrentAbilityState == PlayerController.AbilityState.Grow)
            {
                transform.localScale += new Vector3(_growSpeed * Time.fixedDeltaTime, _growSpeed * Time.fixedDeltaTime, _growSpeed * Time.fixedDeltaTime);
            }
            else if (PlayerController.Instance.CurrentAbilityState == PlayerController.AbilityState.Shrink)
            {
                transform.localScale -= new Vector3(_shrinkSpeed * Time.fixedDeltaTime, _shrinkSpeed * Time.fixedDeltaTime, _shrinkSpeed * Time.fixedDeltaTime);
            }
        }

    }
}
