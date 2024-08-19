using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupEffect : MonoBehaviour
{
    public bool hoveredOver = false;
    
    public bool pickingUp = false;
    
    public bool reachedPlayer = false;
    
    float pickupSpeed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hoveredOver && Input.GetMouseButtonDown(0) && transform.parent == null)
        {
            pickingUp = true;
        }
        
        if( pickingUp && !reachedPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 0.5f - (Camera.main.transform.up * 0.2f), pickupSpeed * Time.deltaTime);
            
            if(Vector3.Distance(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 0.5f - (Camera.main.transform.up * 0.2f)) < 0.1f)
            {
                reachedPlayer = true;
                pickingUp = false;
                transform.parent = Camera.main.transform;
                // If has child turn off all childs colliders:
                if(transform.childCount > 0)
                {
                    foreach(Transform child in transform)
                    {
                        child.GetComponent<Collider>().enabled = false;
                    }
                }
            }
        }
    }
    
    public void ResetValues()
    {
        pickingUp = false;
        reachedPlayer = false;
        hoveredOver = false;
        
        // Raycast to mouse if hits the gameobject, set hoveredOver to true
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 10f))
        {
            if(hit.transform == transform)
            {
                hoveredOver = true;
            }
        }
    }
    
    private void OnMouseEnter()
    {
        if(FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }
        reachedPlayer = false;
        hoveredOver = true;
    }
    
    private void OnMouseExit()
    {
        if(FirstPersonController.Instance.transform.childCount > 0)
        {
            return;
        }
        reachedPlayer = false;
        hoveredOver = false;
    }
}
