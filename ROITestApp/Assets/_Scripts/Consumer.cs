using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Consumer : MonoBehaviour
{
    static int count=0;
    public int internalId;
    public Text inventoryText;
    private int inventory;

    //trucks
    public Transform truckContainer;
    public Transform truckPrefab;

    // Start is called before the first frame update
    void Start()
    {
        truckContainer = GameObject.Find("Trucks").transform;
        internalId = count;
        count++;
        inventory = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncInventory()
    {
        inventory++;
        inventoryText.text = inventory.ToString();
    }

    public void DispatchVehicle(Producer p)
    {
        print("dispatching vehicle from Consumer: " + internalId + " to Producer: " + p.internalId);
        Transform newTruck = Instantiate(truckPrefab, transform.position, Quaternion.identity, truckContainer);
        Truck mTruck = newTruck.GetComponent<Truck>();
        mTruck.targetProducer = p;
        mTruck.targetConsumer = this;
        //get route
        newTruck.GetComponent<Truck>().target = new Vector2Int(Mathf.RoundToInt(p.transform.position.x / GameSpawner.worldScale), 
                                Mathf.RoundToInt(p.transform.position.z / GameSpawner.worldScale));
    }
}
