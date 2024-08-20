using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedVolumeIncrease : MonoBehaviour
{
    public AudioSource audioSource;
    
    public float targetVolume = 0.5f;
    
    bool increasing = false;
    // Start is called before the first frame update
    void Start()
    {
        IncreaseVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(increasing)
        {
            audioSource.volume += 1f * Time.deltaTime;
            if(audioSource.volume >= targetVolume)
            {
                increasing = false;
            }
        }
    }
    
    public void IncreaseVolume()
    {
        StartCoroutine(IncreaseVolumeCoroutine());
    }
    
    private IEnumerator IncreaseVolumeCoroutine()
    {
        yield return new WaitForSeconds(45f);
        increasing = true;
    }
    
}
