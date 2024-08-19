using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance;

    public List<GameObject> CustomersInLine = new List<GameObject>();
    
    public List<GameObject> CustomersWaiting = new List<GameObject>();
    
    public List<GameObject> CustomerPrefabs = new List<GameObject>();
    
    public Sprite BurgerIcon;
    
    public Sprite TacoIcon;
    
    public Sprite HotdogIcon;
    
    public Sprite DrinkIcon;
    
    public Sprite PopsicleIcon;
    
    public GameObject Ticket1;
    
    public SpriteRenderer Ticket1Item1Icon;
    
    public TMP_Text Ticket1Item1Line1Text;
    
    public TMP_Text Ticket1Item1Line2Text;
    
    public TMP_Text Ticket1Item1Line3Text;
    
    public SpriteRenderer Ticket1Item2Icon;
    
    public TMP_Text Ticket1Item2Line1Text;
    
    public TMP_Text Ticket1Item2Line2Text;
    
    public TMP_Text Ticket1Item2Line3Text;
    
    public SpriteRenderer Ticket1Item3Icon;
    
    public TMP_Text Ticket1Item3Line1Text;
    
    public TMP_Text Ticket1Item3Line2Text;
    
    public TMP_Text Ticket1Item3Line3Text;
    
    public GameObject Ticket2;
    
    public SpriteRenderer Ticket2Item1Icon;
    
    public TMP_Text Ticket2Item1Line1Text;
    
    public TMP_Text Ticket2Item1Line2Text;
    
    public TMP_Text Ticket2Item1Line3Text;
    
    public SpriteRenderer Ticket2Item2Icon;
    
    public TMP_Text Ticket2Item2Line1Text;
    
    public TMP_Text Ticket2Item2Line2Text;
    
    public TMP_Text Ticket2Item2Line3Text;
    
    public SpriteRenderer Ticket2Item3Icon;
    
    public TMP_Text Ticket2Item3Line1Text;
    
    public TMP_Text Ticket2Item3Line2Text;
    
    public TMP_Text Ticket2Item3Line3Text;
    
    public GameObject Ticket3;
    
    public SpriteRenderer Ticket3Item1Icon;
    
    public TMP_Text Ticket3Item1Line1Text;
    
    public TMP_Text Ticket3Item1Line2Text;
    
    public TMP_Text Ticket3Item1Line3Text;
    
    public SpriteRenderer Ticket3Item2Icon;
    
    public TMP_Text Ticket3Item2Line1Text;
    
    public TMP_Text Ticket3Item2Line2Text;
    
    public TMP_Text Ticket3Item2Line3Text;
    
    public SpriteRenderer Ticket3Item3Icon;
    
    public TMP_Text Ticket3Item3Line1Text;
    
    public TMP_Text Ticket3Item3Line2Text;
    
    public TMP_Text Ticket3Item3Line3Text;

    public int CustomerCount = 0;
    
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
        
        UpdateAllTickets();
    }
    
    public void UpdateAllTickets()
    {
        UpdateTicketUI(0);
        UpdateTicketUI(1);
        UpdateTicketUI(2);
    }
    
    public void UpdateTicketUI(int customerIndex)
    {
        
        // Assign the correct ticket and UI elements based on the customerIndex
        GameObject ticket = null;
        SpriteRenderer[] icons = null;
        TMP_Text[] line1Texts = null;
        TMP_Text[] line2Texts = null;
        TMP_Text[] line3Texts = null;

        switch (customerIndex)
        {
            case 0:
                ticket = Ticket1;
                icons = new SpriteRenderer[] { Ticket1Item1Icon, Ticket1Item2Icon, Ticket1Item3Icon };
                line1Texts = new TMP_Text[] { Ticket1Item1Line1Text, Ticket1Item2Line1Text, Ticket1Item3Line1Text };
                line2Texts = new TMP_Text[] { Ticket1Item1Line2Text, Ticket1Item2Line2Text, Ticket1Item3Line2Text };
                line3Texts = new TMP_Text[] { Ticket1Item1Line3Text, Ticket1Item2Line3Text, Ticket1Item3Line3Text };
                break;
            case 1:
                ticket = Ticket2;
                icons = new SpriteRenderer[] { Ticket2Item1Icon, Ticket2Item2Icon, Ticket2Item3Icon };
                line1Texts = new TMP_Text[] { Ticket2Item1Line1Text, Ticket2Item2Line1Text, Ticket2Item3Line1Text };
                line2Texts = new TMP_Text[] { Ticket2Item1Line2Text, Ticket2Item2Line2Text, Ticket2Item3Line2Text };
                line3Texts = new TMP_Text[] { Ticket2Item1Line3Text, Ticket2Item2Line3Text, Ticket2Item3Line3Text };
                break;
            case 2:
                ticket = Ticket3;
                icons = new SpriteRenderer[] { Ticket3Item1Icon, Ticket3Item2Icon, Ticket3Item3Icon };
                line1Texts = new TMP_Text[] { Ticket3Item1Line1Text, Ticket3Item2Line1Text, Ticket3Item3Line1Text };
                line2Texts = new TMP_Text[] { Ticket3Item1Line2Text, Ticket3Item2Line2Text, Ticket3Item3Line2Text };
                line3Texts = new TMP_Text[] { Ticket3Item1Line3Text, Ticket3Item2Line3Text, Ticket3Item3Line3Text };
                break;
        }
        
        // if customer index is greater than the number of customers waiting, return
        if (customerIndex >= CustomersWaiting.Count)
        {
            ticket.SetActive(false);
            return;
        }
        else
        {
            ticket.SetActive(true);
        }
        GameObject customer = CustomersWaiting[customerIndex];
        CustomerComponent customerComponent = customer.GetComponent<CustomerComponent>();

        if (customerComponent == null || customerComponent.OrderItems.Count == 0)
        {
            return;
        }

        // Loop through the order items and update the ticket UI
        for (int i = 0; i < customerComponent.OrderItems.Count && i < icons.Length; i++)
        {
            var orderItem = customerComponent.OrderItems[i];

            // Set the icon based on the food type
            switch (orderItem.CurrentFoodType)
            {
                case FoodItemData.FoodType.Burger:
                    icons[i].sprite = BurgerIcon;
                    break;
                case FoodItemData.FoodType.Taco:
                    icons[i].sprite = TacoIcon;
                    break;
                case FoodItemData.FoodType.Hotdog:
                    icons[i].sprite = HotdogIcon;
                    break;
                case FoodItemData.FoodType.Drink:
                    icons[i].sprite = DrinkIcon;
                    break;
                case FoodItemData.FoodType.EmptyDrink:
                    icons[i].sprite = DrinkIcon;
                    break;
                case FoodItemData.FoodType.Popsicle:
                    icons[i].sprite = PopsicleIcon;
                    break;
            }

            // Update the texts
            line1Texts[i].text = orderItem.CurrentOrderSize.ToString();
            line2Texts[i].text = orderItem.CurrentTemperature.ToString();
            if(orderItem.CurrentFoodType == FoodItemData.FoodType.Drink || orderItem.CurrentFoodType == FoodItemData.FoodType.EmptyDrink)
            {
                line3Texts[i].text = orderItem.CurrentDrinkType.ToString();
            }
            else
            {
                line3Texts[i].text = "";
            }
        }

        // Show the ticket
        ticket.SetActive(true);
    }


    IEnumerator SpawnCustomer()
    {
        while (true)
        {
            while(CustomersInLine.Count >= 3)
            {
                yield return null;
            }
        
            int randomIndex = UnityEngine.Random.Range(0, CustomerPrefabs.Count);
            
            GameObject customer = Instantiate(CustomerPrefabs[randomIndex], new Vector3(13.5f, 0.1f, 5.5f), Quaternion.identity);
            
            CustomerCount++;
            
            CustomerComponent customerComponent = customer.GetComponent<CustomerComponent>();
            customerComponent.CustomerNumber = CustomerCount;
            
            CustomersInLine.Add(customer);
        
        
            yield return new WaitForSeconds(TimeBetweenCustomers);
        
            TimeBetweenCustomers -= 1;
        }

    }
}
