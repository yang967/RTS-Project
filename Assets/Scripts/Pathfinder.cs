using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class Pathfinder
{
    struct initializer : IJobParallelFor
    {
        public NativeArray<int> arr;

        public void Execute(int index)
        {
            arr[index] = int.MaxValue;
        }
    }

    //List<int> openSet;
    MapHeap openSet;
    bool[] inOpenSet;
    int[] hasObstacle;
    NativeArray<int> gScore;
    NativeArray<int> fScore;
    List<Vector3> path;
    Vector3 target;
    FlowField ff;

    int close;
    int min_distance;

    public Pathfinder(Vector3 start, Vector3 end, int gridLength, bool considerUnit)
    {
        float time = Time.realtimeSinceStartup;
        float minTime = 0;
        //openSet = new List<int>();
        ff = GameObject.Find("EventSystem").GetComponent<FlowField>();
        openSet = new MapHeap();
        target = new Vector3(end.x, end.y, end.z);
        int length = 1000 / gridLength;
        int width = 1000 / gridLength;
        gScore = new NativeArray<int>(length * width, Allocator.Persistent);
        fScore = new NativeArray<int>(length * width, Allocator.Persistent);
        inOpenSet = new bool[length * width];
        hasObstacle = new int[length * width];
        hasObstacle = ff.getCostField();
        min_distance = int.MaxValue;
        initializer initializeGScore = new initializer
        {
            arr = gScore
        };
        JobHandle gScoreHandler = initializeGScore.Schedule(length * width, 100);
        gScoreHandler.Complete();
        initializer initializeFScore = new initializer
        {
            arr = fScore
        };
        JobHandle fScoreHandler = initializeFScore.Schedule(length * width, 100);
        fScoreHandler.Complete();
        int current = Normalize(start, gridLength, width);
        close = current;
        int goal = Normalize(end, gridLength, width);
        int[] cameFrom = new int[length * width];
        cameFrom[current] = current;
        gScore[current] = 0;
        openSet.push(current, fScore);
        //openSet.Add(current);
        inOpenSet[current] = true;
        int tentativeScore;

        //int cx, cy;

        //Debug.Log("Initialization time: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        time = Time.realtimeSinceStartup;
        float t;
        int x, y, neighbor;
        int processed = 0;
        while(openSet.Count() > 0)
        {
            if (gridLength != ff.getGridLength() && processed > 100)
                break;
            t = Time.realtimeSinceStartup;
            //current = getMin();
            current = openSet.pop(fScore);
            minTime += Time.realtimeSinceStartup - t;
            if (current == goal)
            {
                cameFrom[goal] = cameFrom[current];
                break;
            }

            //openSet.Remove(current);
            inOpenSet[current] = false;
            //neighbors = getSurrounding(current, length, width, gridLength, considerUnit);

            x = current / width;
            y = current % width;

            //foreach (int neighbor in neighbors)

            for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(length - 1, x + 1); i += 1)
                for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(width - 1, y + 1); j += 1)
                {
                    neighbor = i * width + j;
                    /*Debug.Log(i + " " + j + " | " + FlowField.IndexToFloat3(neighbor, gridLength, width).x + " " + FlowField.IndexToFloat3(neighbor, gridLength, width).z);
                    if (i != j && !inOpenSet[neighbor])
                    {
                        GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cx = i * gridLength - 500;
                        cy = j * gridLength - 500;
                        gameobj.transform.position = new Vector3(cx, 10, cy);
                        if ((gridLength == ff.getGridLength() ? hasObstacle[neighbor] == 65535 : HasObstacle.hasObstacle(FlowField.IndexToFloat3(neighbor, gridLength, width), gridLength, true)))
                            gameobj.GetComponent<MeshRenderer>().material.color = Color.red;
                    }*/

                    if (i == j || (gridLength == ff.getGridLength() ? hasObstacle[neighbor] == 65535 : HasObstacle.hasObstacle(FlowField.IndexToFloat3(neighbor, gridLength, width), gridLength, considerUnit)))
                        continue;
                    tentativeScore = gScore[current] + getDistance(current, neighbor, width);
                    if (gScore[neighbor] > tentativeScore)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeScore;
                        fScore[neighbor] = tentativeScore + getDistance(neighbor, goal, width);
                        if(getDistance(current, goal, width) < min_distance)
                        {
                            min_distance = getDistance(current, goal, width);
                            close = current;
                        }
                        if (!inOpenSet[neighbor])
                        {
                            openSet.push(neighbor, fScore);
                            //openSet.Add(neighbor);
                            inOpenSet[neighbor] = true;
                            processed++;
                        }
                    }
                }   
        }
        if(current == goal)
            generatePath(cameFrom, gridLength, goal, width);
        else
            generatePath(cameFrom, gridLength, close, width);
        fScore.Dispose();
        gScore.Dispose();
        //Debug.Log("Calculation time: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        //Debug.Log("getMin() time: " + minTime * 1000 + "ms");
    }

    public Vector3 getTarget()
    {
        return target;
    }

    void generatePath(int[] cameFrom, int gridLength, int goal, int width)
    {
        path = new List<Vector3>();
        int current = goal;
        while (cameFrom[current] != current)
        {
            path.Add(CoordinateToVector3(current, gridLength, width));
            current = cameFrom[current];
        }
    }

    public List<Vector3> getPath()
    {
        return path;
    }

    /*int getMin()
    {
        int result = openSet[0];
        for(int i = 1; i < openSet.Count; i++)
        {
            if (fScore[result] > fScore[openSet[i]])
                result = openSet[i];
        }
        return result;
    }*/

    int getDistance(int coordinate1, int coordinate2, int width)
    {
        int x1 = coordinate1 / width;
        int y1 = coordinate1 % width;
        int x2 = coordinate2 / width;
        int y2 = coordinate2 % width;
        int x = Mathf.Abs(x1 - x2);
        int y = Mathf.Abs(y1 - y2);
        return (Mathf.Max(x, y) - Mathf.Min(x, y)) * 10 + Mathf.Min(x, y) * 14;
    }

    List<int> getSurrounding(int Coordinate, int length, int width, int gridLength, bool considerUnit)
    {
        List<int> result = new List<int>();
        int neighbor;
        int index = 0;
        int x = Coordinate / width;
        int y = Coordinate % width;
        for (int i = Mathf.Max(0, x - 1); i <= Mathf.Min(length - 1, x + 1); i += 1)
            for (int j = Mathf.Max(0, y - 1); j <= Mathf.Min(width - 1, y + 1); j += 1)
            {
                index++;
                if (i == x && j == y)
                    continue;
                neighbor = i * width + j;
                /*if(hasObstacle[neighbor] == 0)
                {
                    if (HasObstacle.hasObstacle(new float3(i * gridLength - 500, 0, j * gridLength - 500), gridLength, considerUnit))
                        hasObstacle[neighbor] = 2;
                    else
                        hasObstacle[neighbor] = 1;
                }*/
                if (hasObstacle[neighbor] != 65535)
                    result.Add(neighbor);
            }
        return result;
    }

    int Normalize(Vector3 position, int gridLength, int width)
    {
        int x = Mathf.RoundToInt((position.x + 500) / gridLength), y = Mathf.RoundToInt((position.z + 500) / gridLength);
        return x * width + y;
    }

    Vector3 CoordinateToVector3(int Coordinate, int gridLength, int width)
    {
        int x = (Coordinate / width) * gridLength - 500;
        int y = (Coordinate % width) * gridLength - 500;
        Vector3 result = new Vector3(x, 0, y);
        result.y = Terrain.activeTerrain.SampleHeight(result);
        return result;
    }
}
