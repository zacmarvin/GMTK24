using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollLeftRectTransform : MonoBehaviour
{
    public float ScrollSpeed = 1f;
    
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        
        
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        rectTransform.anchoredPosition += new Vector2(ScrollSpeed * Time.deltaTime, 0);
        
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(55f);
        Destroy(gameObject);
    }
}
