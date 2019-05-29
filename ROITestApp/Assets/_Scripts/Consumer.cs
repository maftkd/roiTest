//Consumer.cs
//
//Description: Represents buildings that accept consume inventory of the producer. 
//In order to acquire inventory, the consumers are responsible for dispatching vehicles. See DispatchVehicle below
//
//Created by: Michael Feldman
//Date: 5-29-2019

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Consumer : MonoBehaviour
{
    static int count=0;
    public int internalId;
    public Text inventoryText;
    private int inventory;

    public Queue<Vector2Int> path;
    //trucks
    public Transform truckContainer;
    public Transform truckPrefab;
    public Transform truckPool;

    // Start is called before the first frame update
    void Start()
    {
        truckContainer = GameObject.Find("Trucks").transform;
        internalId = count;
        count++;
        inventory = 0;
        truckPool = GameObject.Find("TruckPool").transform;
    }

    public void IncInventory()
    {
        inventory++;
        inventoryText.text = inventory.ToString();
    }

    public void DispatchVehicle(Producer p)
    {
        //create a new truck at the consumers location
        //later we can refactor this to utilize an object pool
        //Transform newTruck = Instantiate(truckPrefab, transform.position, Quaternion.identity, truckContainer);
        Transform newTruck = truckPool.GetChild(0);
        newTruck.position = transform.position;
        newTruck.SetParent(truckContainer);
        Truck mTruck = newTruck.GetComponent<Truck>();
        mTruck.targetProducer = p;
        mTruck.targetConsumer = this;
        Vector2Int dest = new Vector2Int(Mathf.RoundToInt(p.transform.position.x / GameSpawner.worldScale),
                        Mathf.RoundToInt(p.transform.position.z / GameSpawner.worldScale));
        mTruck.SetDestinationPath(path,dest);
    }

    //modifier method called from GameSpawner during initialization
    public void SetPath(Queue<Vector2Int> mPath)
    {
        if(path==null)
            path = mPath;
    }
}
