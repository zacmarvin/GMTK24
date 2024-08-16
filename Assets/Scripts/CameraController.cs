using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _offset;

    [SerializeField]
    private float _smoothSpeed = 0.125f;

    private void Start()
    {
        transform.position = _target.position + _target.TransformDirection(_offset);
        transform.rotation = _target.rotation;
    }

    void FixedUpdate()
    {
        // Calculate the desired position of the camera
        Vector3 desiredPosition = _target.position + _target.TransformDirection(_offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        // Move the camera to the smoothed position
        transform.position = smoothedPosition;

        // Make the camera look in the direction the player is facing
        transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, _smoothSpeed * Time.deltaTime);
    }
}