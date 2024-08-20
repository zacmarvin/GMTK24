using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgruntledAudioPlayer : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OccosionallyPlaySound());
        audioSource = GetComponent<AudioSource>();
    }
    
    IEnumerator OccosionallyPlaySound()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(10f, 30f));
            if(CustomerManager.Instance.CustomersWaiting.Count > 0)
            {
                if(audioClips.Count > 0)
                {
                    AudioClip clipToPlay = audioClips[Random.Range(0, audioClips.Count)];
                    audioSource.PlayOneShot(clipToPlay);
                }
            }
        }
    }
}
