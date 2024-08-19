using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHinge : MonoBehaviour
{

    [SerializeField] private string axis = "y";
    bool currentlyRotating = false;
    
    public void ToggleDoor()
    {
        if(currentlyRotating)
        {
            return;
        }
        
        // If door is open, close it
        if(axis == "x" && transform.rotation.eulerAngles.x > 0)
        {
            CloseDoor();
        }
        else if(axis == "y" && transform.rotation.eulerAngles.y > 0)
        {
            CloseDoor();
        }
        else if(axis == "z" && transform.rotation.eulerAngles.z > 0)
        {
            CloseDoor();
        }
        // If door is closed, open it
        else
        {
            OpenDoor();
        }
    }
    
    public void OpenDoor()
    {
        // Rotate door 90 degrees on x axis over 1 second
        StartCoroutine(RotateDoor(90, 1));
    }
    
    public void CloseDoor()
    {
        // Rotate door -90 degrees on x axis over 1 second
        StartCoroutine(RotateDoor(-90, 1));
    }
    
    IEnumerator RotateDoor(float angle, float duration)
    {
        currentlyRotating = true;
        Quaternion startRotation = transform.rotation;

        Quaternion endRotation = transform.rotation * Quaternion.Euler(0, angle, 0);
        if(axis == "x")
        {
            endRotation = Quaternion.Euler(angle, 0, 0);
        }
        else if(axis == "y")
        {
            endRotation = Quaternion.Euler(0, angle, 0);
        }
        else if(axis == "z")
        {
            endRotation = Quaternion.Euler(0, 0, angle);
        }
        float startTime = Time.time;
        
        while(Time.time < startTime + duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, (Time.time - startTime) / duration);
            yield return null;
        }
        
        transform.rotation = endRotation;
        currentlyRotating = false;
    }
}
