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

    private Stack<Vector2Int> directions = new Stack<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        myPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / GameSpawner.worldScale), Mathf.RoundToInt(transform.position.z / GameSpawner.worldScale));
        //should be given target upon instantiation
        StartCoroutine(TravelTo(target));
    }

    private IEnumerator TravelTo(Vector2Int dest)
    {
        if (!goingHome)
        {
            mapData = GameObject.Find("Spawner").transform.GetComponent<GameSpawner>().mapData;
            mapSize = GameObject.Find("Spawner").transform.GetComponent<GameSpawner>().mapSize;
            maxDistanceSqr = GameObject.Find("Spawner").transform.GetComponent<GameSpawner>().maxDistanceSqr;
            //while we are not at target
            //get target
            //logic stuff
            //move ot target
            //check neighboring tiles, find which one has the shortest path to target
            float[] paths = new float[4];
            Vector2 nextPos = Vector2.zero;
            Vector2 tmpPos;
            float tmpMin;
            int minIndex;

            while ((dest - myPos).sqrMagnitude > 1.1f)
            {
                //temp code just have the trucks float up if they aren't within range
                yield return new WaitForSeconds(.1f);

                //check north
                if (myPos.y < mapSize.y - 1 && ((mapData[myPos.x][myPos.y + 1] & 3) == 2))
                {
                    tmpPos = myPos + Vector2.up;
                    paths[0] = (dest - tmpPos).sqrMagnitude;
                    nextPos = tmpPos;
                }
                else
                    paths[0] = maxDistanceSqr;
                //check east
                if (myPos.x < mapSize.x - 1 && ((mapData[myPos.x + 1][myPos.y] & 3) == 2))
                {
                    tmpPos = myPos + Vector2.right;
                    paths[1] = (dest - tmpPos).sqrMagnitude;
                    nextPos = tmpPos;
                }
                else
                    paths[1] = maxDistanceSqr;
                //check south
                if (myPos.y > 0 && ((mapData[myPos.x][myPos.y - 1] & 3) == 2))
                {
                    tmpPos = myPos + Vector2.down;
                    paths[2] = (dest - tmpPos).sqrMagnitude;
                    nextPos = tmpPos;
                }
                else
                    paths[2] = maxDistanceSqr;
                //check west
                if (myPos.x > 0 && ((mapData[myPos.x - 1][myPos.y] & 3) == 2))
                {
                    tmpPos = myPos + Vector2.left;
                    paths[3] = (dest - tmpPos).sqrMagnitude;
                    nextPos = tmpPos;
                }
                else
                    paths[3] = maxDistanceSqr;

                //find min distance to determine optimal next direction
                tmpMin = maxDistanceSqr;
                minIndex = -1;
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i] < tmpMin)
                    {
                        tmpMin = paths[i];
                        minIndex = i;
                    }
                }

                //Using min distance determine the next position in xy space   
                if (minIndex == 0)
                {
                    myPos += Vector2Int.up;
                    transform.position += Vector3.forward * GameSpawner.worldScale;
                    directions.Push(Vector2Int.down);
                }
                else if (minIndex == 1)
                {
                    myPos += Vector2Int.right;
                    transform.position += Vector3.right * GameSpawner.worldScale;
                    directions.Push(Vector2Int.left);
                }
                else if (minIndex == 2)
                {
                    myPos += Vector2Int.down;
                    transform.position += Vector3.back * GameSpawner.worldScale;
                    directions.Push(Vector2Int.up);
                }
                else if (minIndex == 3)
                {
                    myPos += Vector2Int.left;
                    transform.position += Vector3.left * GameSpawner.worldScale;
                    directions.Push(Vector2Int.right);
                }
                else
                {
                    Debug.Log("Error - truck could not find path");
                }
            }
            goingHome = true;
            targetProducer.GiveProduct();
            Vector2Int newTarget = new Vector2Int(Mathf.RoundToInt(targetConsumer.transform.position.x / .1f),
                                Mathf.RoundToInt(targetConsumer.transform.position.z / .1f));
            StartCoroutine(TravelTo(newTarget));
        }
        else
        {
            
            while ((dest - myPos).sqrMagnitude > 1.1f)
            {
                yield return new WaitForSeconds(.1f);
                Vector2Int dir = directions.Pop();
                myPos += dir;
                transform.position += new Vector3(dir.x * GameSpawner.worldScale, 0, dir.y * GameSpawner.worldScale);
            }
            targetConsumer.IncInventory();
            Destroy(transform.gameObject);

        }
    }
}
