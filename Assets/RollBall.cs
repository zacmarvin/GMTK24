using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBall : MonoBehaviour
{
    public bool IsRolling = false;

    public float RollingSpeed = 1f;
    
    private Rigidbody _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(IsRolling)
        {
            _rigidbody.AddForce(Vector3.right * RollingSpeed);
            // Angular velocity is the speed at which an object rotates
            _rigidbody.angularVelocity = new Vector3(0, 0, -RollingSpeed);
        }
    }
    
    public void StartRolling()
    {
        IsRolling = true;
    }
}
