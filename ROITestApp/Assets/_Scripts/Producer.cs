//Producer.cs
//
//Description: Continuously produces products and dispatches consumers to pick up in a cyclical mannor
//Contains a coroutine which delays production by the var: productionDelay
//Updates an gui to show inventory and remaining production delay time.
//
//Created by: Michael Feldman
//Date: 5-29-2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Producer : MonoBehaviour
{
    static int count = 0;
    public int internalId;

    public Image productionTimerSprite;
    public Text inventoryText;
    private float productionTimer;
    private int inventory;
    public float productionDelay;

    private List<Transform> consumers = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {

        internalId = count;
        count++;
        StartCoroutine(GenerateProduct());
    }

    private IEnumerator GenerateProduct()
    {
        while (productionTimer < productionDelay)
        {
            productionTimer += Time.deltaTime;

            if(productionTimer >= productionDelay)
            {
                inventory++;
                productionTimer = 0;
                productionTimerSprite.fillAmount = 0;
                inventoryText.text = inventory.ToString();
                //Dispatch consumer to pick up
                DispatchNextConsumer();
            }
            else
            {
                productionTimerSprite.fillAmount = productionTimer / productionDelay;
            }
            yield return null;
        }        
    }

    [ContextMenu("CeaseProduction")]
    public void CeaseProduction()
    {
        StopAllCoroutines();
        productionTimer = 0;
        productionTimerSprite.fillAmount = 1;
        productionTimerSprite.color = Color.red;
    }

    [ContextMenu("ResumeProduction")]
    public void ResumeProduction()
    {
        
        productionTimer = 0;
        productionTimerSprite.fillAmount = 0;
        productionTimerSprite.color = Color.white;

        StartCoroutine(GenerateProduct());
    }



    private void DispatchNextConsumer()
    {
        //cycle in round robin fashion
        if (consumers.Count > 0)
        {
            //print("dispatching consumer: Consumer length: " + consumers.Count);
            Transform next = consumers[0];
            consumers.RemoveAt(0);
            consumers.Add(next);

            next.GetComponent<Consumer>().DispatchVehicle(this);
        }
        else
            print("No consumers near this producer");
        
    }

    public void AddConsumer(Transform c)
    {
        consumers.Add(c);
    }

    public void GiveProduct()
    {
        inventory--;
        inventoryText.text = inventory.ToString();
    }
}
