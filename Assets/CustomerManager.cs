using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    public List<GameObject> CustomersInLine = new List<GameObject>();
    
    public List<GameObject> CustomersWaiting = new List<GameObject>();
    
    public GameObject CustomerPrefab;
    
    public int TimeBetweenCustomers = 10;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCustomer());
    }
    
    public void WaitForFood(GameObject customer)
    {
        CustomersInLine.Remove(customer);
        CustomersWaiting.Add(customer);
    }

    IEnumerator SpawnCustomer()
    {
        while (true)
        {
            while(CustomersInLine.Count >= 3)
            {
                yield return null;
            }
        
            GameObject customer = Instantiate(CustomerPrefab, new Vector3(13.5f, 1, 5.5f), Quaternion.identity);
        
            CustomersInLine.Add(customer);
        
        
            yield return new WaitForSeconds(TimeBetweenCustomers);
        
            TimeBetweenCustomers -= 1;
        }

    }
}
