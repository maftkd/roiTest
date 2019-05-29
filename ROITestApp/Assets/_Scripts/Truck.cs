//Truck.cs
//
//Description: The truck is the transporter of goods between producer and consumer.
//In general the truck does not perform pathfinding, simply backtracking
//The truck receives a queue of directions (it's a queue because it's assembled in order - see GameSpawner.cs)
//Utilizes the instructions step by step and saves the inverse to a stack: directions Home in order to return
//to a consumer
//
//Created by: Michael Feldman
//Date: 5-29-2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    Vector2Int myPos;
    public Vector2Int target;
    public Producer targetProducer;
    public Consumer targetConsumer;
    private byte[][] mapData;
    private Vector2Int mapSize;
    private float maxDistanceSqr;

    private bool goingHome = false;

    public Queue<Vector2Int> directionsThere;
    private Stack<Vector2Int> directionsHome = new Stack<Vector2Int>();

    public Transform truckPool;

    // Start is called before the first frame update
    void Start()
    {
        truckPool = GameObject.Find("TruckPool").transform;
    }

    //temp vectors for animation
    private Vector3 oldPos, newPos;
    private IEnumerator TravelTo(Vector2Int dest)
    {        

        //if truck is going to producer to fetch product
        if (!goingHome)
        {
            //update until the truck's position (myPos) has reached the destination
            while ((dest - myPos).sqrMagnitude > 1.1f)
            {
                //get the next tile direction
                Vector2Int dir = directionsThere.Dequeue();
                //save the reverse so we know how to get back
                directionsHome.Push(new Vector2Int(dir.x*-1,dir.y*-1));
                //move in world space and in the game world (at least the indices are being tracked)
                myPos += dir;
                oldPos = transform.position;
                newPos = transform.position + new Vector3(dir.x * GameSpawner.worldScale, 0, dir.y * GameSpawner.worldScale);
                for (float i=.25f; i<=1; i+=0.25f)
                {
                    transform.position = Vector3.Lerp(oldPos, newPos, i);
                    yield return null;
                }
                
            }
            goingHome = true;
            targetProducer.GiveProduct();
            //turn around and deliver product
            Vector2Int newTarget = new Vector2Int(Mathf.RoundToInt(targetConsumer.transform.position.x / .1f),
                                Mathf.RoundToInt(targetConsumer.transform.position.z / .1f));
            StartCoroutine(TravelTo(newTarget));
        }
        else
        {            
            while ((dest - myPos).sqrMagnitude > 1.1f)
            {
                Vector2Int dir = directionsHome.Pop();
                myPos += dir;
                oldPos = transform.position;
                newPos = transform.position + new Vector3(dir.x * GameSpawner.worldScale, 0, dir.y * GameSpawner.worldScale);
                for (float i = .25f; i <= 1; i += 0.25f)
                {
                    transform.position = Vector3.Lerp(oldPos, newPos, i);
                    yield return null;
                }
            }
            //increase inventory then self-destruct
            print("truck has reached consumer");
            targetConsumer.IncInventory();
            //Destroy(transform.gameObject);
            //we can change this to put the truck in the truckpool
            transform.SetParent(truckPool);
            transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
            transform.position = Vector3.down;
            StopAllCoroutines();
        }
    }

    public void SetDestinationPath(Queue<Vector2Int> path, Vector2Int dest)
    {
        goingHome = false;
        directionsHome = new Stack<Vector2Int>();
        //get directions
        directionsThere = new Queue<Vector2Int>(path);
        //turn on trail renderer here for kicks
        transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;

        //get my position
        myPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / GameSpawner.worldScale), Mathf.RoundToInt(transform.position.z / GameSpawner.worldScale));
        StartCoroutine(TravelTo(dest));
    }
}
