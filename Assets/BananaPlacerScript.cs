using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPlacerScript : MonoBehaviour
{
    [SerializeField] private GameObject _bananaPrefab;
    
    [SerializeField] private GameObject _bananaGhostPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Raycast from the camera to the mouse cursor
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.transform == transform)
            {
                
                // Show transparent banana at hit point
                _bananaGhostPrefab.transform.position = hit.point;
                
                // If the raycast hits a collider, place a banana at the hit point
                if (Input.GetMouseButtonDown(0))
                {
                    _bananaPrefab.transform.localScale = _bananaGhostPrefab.transform.localScale;
                    Rigidbody rb = _bananaPrefab.GetComponent<Rigidbody>();
                    rb.mass = _bananaGhostPrefab.transform.localScale.x * 3.33f;
                    Instantiate(_bananaPrefab, hit.point, _bananaGhostPrefab.transform.rotation);
                }
            }
        }
        
        // Scroll wheel to rotate banana
        if (Input.mouseScrollDelta.y > 0)
        {
            // If left control is held down, scale the banana
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _bananaGhostPrefab.transform.localScale += new Vector3(0.02f, 0.02f, 0.02f);
                // if too large scale, set to 3
                if (_bananaGhostPrefab.transform.localScale.x > 0.5f)
                {
                    _bananaGhostPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }

            }
            else
            {
                _bananaGhostPrefab.transform.Rotate(Vector3.forward, 10f);
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            // If left control is held down, scale the banana
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _bananaGhostPrefab.transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
                // if negative scale, set to 0.1
                if (_bananaGhostPrefab.transform.localScale.x < 0.05f)
                {
                    _bananaGhostPrefab.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                }
            }
            else
            {
                _bananaGhostPrefab.transform.Rotate(Vector3.forward, -10f);
            }
            
        }
    }
}
