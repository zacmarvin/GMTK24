using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.parent == FirstPersonController.Instance.transform)
        {
            if(Input.GetMouseButtonDown(1))
            {
                transform.parent = null;
                transform.gameObject.AddComponent<Rigidbody>();
                    
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10f, ForceMode.Impulse);
            }
        }
    }
}
