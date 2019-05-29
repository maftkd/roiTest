﻿//GameSpawner.cs
//
//Description: Spawns producers, consumers, and roads. Performs A* Path finding to connect producers and consumers with roads
//
//byte[][] mapData: This 2D byte array represents all available tiles on the map. bytes of 0 represent an available tile, while
//1 represents a producer
//2 represents a road (it matters that the least significant bit is a 0)
//3 represents a consumer
//
//Created by: Michael Feldman
//Date: 5-29-2019

using System.Collections;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    public Vector2Int mapSize;
    public float maxDistanceSqr;
    public byte[][] mapData;
    public int producerCount, consumerCount;
    public Transform producer, consumer, road;
    public Transform producerContainer, consumerContainer, roadContainer;
    public static float worldScale =.1f;
    private float worldScaleSquared;
    // Start is called before the first frame update
    void Start()
    {
        SpawnWorld();
    }

    //handles logic of spawning all items
    [ContextMenu("ReSpawn")]
    public void SpawnWorld()
    {
        //initialize map of size 256 x 256 with all 0's for empty slots
        mapData = new byte[mapSize.x][];
        for(int i=0; i<mapData.Length; i++)
        {
            mapData[i] = new byte[mapSize.y];
            for(int j=0; j<mapSize.y; j++)
            {
                mapData[i][j] = 0;
            }
        }

        //spawn our producers next
        SpawnBuildingRandom(producer, producerContainer, producerCount, 1);

        //next the consumers
        SpawnBuildingRandom(consumer, consumerContainer, consumerCount, 3);

        //use this to associate producers with consumers by distance
        maxDistanceSqr = mapSize.x * mapSize.x + mapSize.y * mapSize.y;

        //assign the consumers to the nearest producer
        float min = maxDistanceSqr;
        Producer nearest = new Producer(); //placeholder
        foreach(Transform consumer in consumerContainer){
            foreach(Transform producer in producerContainer)
            {
                float distSqr = (producer.position/worldScale - consumer.position/worldScale).sqrMagnitude;
                if (distSqr < min)
                {
                    min = distSqr;
                    nearest = producer.GetComponent<Producer>();
                }
            }
            //actually assign the consumer to be part of the producer network
            nearest.AddConsumer(consumer);
            GenerateRoadFromTo(consumer, nearest.transform, 2, 3, 1,true);
            min = maxDistanceSqr;
        }

        //finally the roads
        //initialize some variables we need for pathfinding
        //worldScaleSquared = Mathf.Pow(worldScale, 2);
        
        /*foreach(Transform consumer in consumerContainer)
        {
            foreach(Transform producer in producerContainer)
            {
                GenerateRoadFromTo(consumer, producer, 2, 3, 1);
            }
        }*/
    }

    private void SpawnBuildingRandom(Transform prefab, Transform prefabContainer, int count, byte tileId)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3Int newPos = new Vector3Int(Random.Range(0, mapSize.x), 0, Random.Range(0, mapSize.y));
            //continue regenerating a spawn point until we find one that has not been taken
            while (mapData[newPos.x][newPos.z] != 0)
            {
                newPos = new Vector3Int(Random.Range(0, mapSize.x), 0, Random.Range(0, mapSize.y));
            }
            Vector3 worldPos = new Vector3((float)newPos.x * worldScale, 0, (float)newPos.z * worldScale);
            Instantiate(prefab, worldPos, Quaternion.identity, prefabContainer);

            //make sure to tell the mapData array where the new building is
            mapData[newPos.x][newPos.z] = tileId;
        }
    }

    private void TestFunction(Transform origin, Transform destination, byte tileId, byte originTileId, byte destinationTileId, bool consToProd = false)
    {

    }
    
    private void GenerateRoadFromTo(Transform origin, Transform destination, byte tileId, byte originTileId, byte destinationTileId, bool consToProd=false)
    {
        Vector2Int current = new Vector2Int(Mathf.RoundToInt(origin.position.x / worldScale), Mathf.RoundToInt(origin.position.z / worldScale));
        Vector2Int final = new Vector2Int(Mathf.RoundToInt(destination.position.x / worldScale), Mathf.RoundToInt(destination.position.z / worldScale));
        byte curTile = originTileId;
        
        //this array stores the distanceSquared for 4 potential directions we can go from any particular time
        //0 represents North, 1 represents East, 2 represents South, 3 represents West
        float[] paths = new float[4];
        Vector2 nextPos = Vector2.zero;
        Vector2 tmpPos;
        float tmpMin;
        int minIndex;
        
        //keep dropping roads until we have reaced our destination
        while ((final-current).sqrMagnitude >1.1f)
        {
            //plop a road
            //check north
            if (current.y < mapSize.y - 1 && ((mapData[current.x][current.y + 1] & 1)==0))
            {
                tmpPos = current + Vector2.up;
                paths[0] = (final - tmpPos).sqrMagnitude;
                nextPos = tmpPos;
            }
            else
                paths[0] = maxDistanceSqr;
            //check east
            if (current.x < mapSize.x - 1 && ((mapData[current.x + 1][current.y] & 1) == 0))
            {
                tmpPos = current + Vector2.right;
                paths[1] = (final - tmpPos).sqrMagnitude;
                nextPos = tmpPos;
            }
            else
                paths[1] = maxDistanceSqr;
            //check south
            if (current.y > 0 && ((mapData[current.x][current.y - 1] & 1) == 0))
            {
                tmpPos = current + Vector2.down;
                paths[2] = (final - tmpPos).sqrMagnitude;
                nextPos = tmpPos;
            }
            else
                paths[2] = maxDistanceSqr;
            //check west
            if (current.x > 0 && ((mapData[current.x - 1][current.y] & 1) == 0))
            {
                tmpPos = current + Vector2.left;
                paths[3] = (final - tmpPos).sqrMagnitude;
                nextPos = tmpPos;
            }
            else
                paths[3] = maxDistanceSqr;

            //find min distance to determine optimal next direction
            tmpMin = maxDistanceSqr;
            minIndex = -1;
            for(int i = 0; i<paths.Length; i++)
            {
                if (paths[i] < tmpMin)
                {
                    tmpMin = paths[i];
                    minIndex = i;
                }
            }

            //Using min distance determine the next position in xy space            
            byte targetId = tileId;
            if (minIndex == 0)
            {
                current += Vector2Int.up;
                targetId = (byte)(targetId | 32); //up to producer down to consumer
            }
            else if (minIndex == 1)
            {
                current += Vector2Int.right;
                targetId = (byte)(targetId | 112); //right to producer left to consumer
            }
            else if (minIndex == 2)
            {
                current += Vector2Int.down;
                targetId = (byte)(targetId | 128); //down to producer left to consumer
            }
            else if (minIndex == 3)
            {
                current += Vector2Int.left;
                targetId = (byte)(targetId | 208); //left to producer left to consumer
            }
            else
            {
                Debug.Log("Error creating path from: " + origin.name + " to " + destination.name);
                break;
            }

            //instantiate the road and add it to our map
            Instantiate(road, new Vector3(current.x * worldScale, -.05f, current.y * worldScale), Quaternion.identity, roadContainer);
            mapData[current.x][current.y] = targetId;

        }
    }
}
