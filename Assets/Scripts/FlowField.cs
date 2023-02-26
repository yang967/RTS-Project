using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public struct float3
{
    public int x;
    public int y;
    public int z;

    public float3(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public bool equals(float3 position)
    {
        if (position.x == x && position.z == z)
            return true;
        return false;
    }

    public float3 up()
    {
        return new float3(0, 1, 0);
    }

    public float3 down()
    {
        return new float3(0, -1, 0);
    }

    public float3 forward()
    {
        return new float3(1, 0, 0);
    }

    public float3 back()
    {
        return new float3(-1, 0, 0);
    }

    public float3 left()
    {
        return new float3(0, 0, -1);
    }

    public float3 right()
    {
        return new float3(0, 0, 1);
    }
}



public class FlowField : MonoBehaviour
{
    int[] costField;
    NativeArray<int> integrationField;
    float3[] flowField;
    static int gridLength;
    int length;
    static int width;
    bool[] obstacle;
    bool generated;
    public bool debugMod;
    public bool test = false;

    float3 start;

    [BurstCompile]
    struct GenerateFlowField : IJobParallelFor
    {
        [ReadOnly]public NativeArray<int> Integration;
        public NativeArray<float3> flowField;
        public int length;
        public int width;
        public int gridlength;

        public void Execute(int index)
        {
            int x, y;
            x = index / width;
            y = index % width;
            int iMin = x - 1 >= 0 ? x - 1 : 0;
            int jMin = y - 1 >= 0 ? y - 1 : 0;
            for(int i = x - 1 >= 0 ? x - 1 : 0; i <= (x + 1 <= length - 1 ? x + 1 : length - 1); i++)
                for(int j = y - 1 >= 0 ? y - 1 : 0; j <= (y + 1 <= width - 1 ? y + 1 : width - 1); j++)
                {
                    if(Integration[i * width + j] < Integration[iMin * width + jMin])
                    {
                        iMin = i;
                        jMin = j;
                    }
                }
            flowField[index] = new float3(iMin * gridlength - 500, 0, jMin * gridlength - 500);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        gridLength = 10;
        generateCostField();
        generated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
            TestCostField();
    }

    public float3 getTarget()
    {
        return start;
    }

    public int getGridLength()
    {
        return gridLength;
    }

    void generateCostField()
    {
        length = 1000 / gridLength;
        width = 1000 / gridLength;
        costField = new int[length * width];
        obstacle = new bool[length * width];
        for(int i = 0; i < length * width; i++)
        {
            if (HasObstacle.hasObstacle(new float3(i / width * gridLength - 500, 2, i % width * gridLength - 500), 10, false))
            {
                costField[i] = 65535;
                obstacle[i] = true;
            }    
            else
            {
                costField[i] = 1;
                obstacle[i] = false;
            }
        }
        //TestCostField();
    }

    public void generate(float3 position)
    {
        start = Normalize(position);
        position = start;
        //float time = Time.realtimeSinceStartup;
        integrationField = new NativeArray<int>(length * width, Allocator.Persistent);
        generateIntegrationField(position);
        //Debug.Log(integrationField[52 * width + 58]);
        generateFlowField(position);
        //Debug.Log("Time: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
    }

    public int[] getCostField()
    {
        return costField;
    }

    public void updateCostField(int index, int value)
    {
        int l, w;
        if (index < costField.Length)
        {
            l = index / width * gridLength - 500;
            w = index % width * gridLength - 500;
            costField[index] = value;
        }
    }

    void generateFlowField(float3 position)
    {
        if(!debugMod)
        {
            NativeArray<float3> flow = new NativeArray<float3>(length * width, Allocator.TempJob);
            GenerateFlowField field = new GenerateFlowField
            {
                length = length,
                width = width,
                gridlength = gridLength,
                Integration = integrationField,
                flowField = flow
            };

            JobHandle jobHandle = field.Schedule(length * width, 100);
            jobHandle.Complete();
            

            flowField = flow.ToArray();
            flow.Dispose();
        }
        else
        {
            int iMin, jMin, x, y;
            GameObject debug;
            flowField = new float3[length * width];
            for(int i = 0; i < length * width; i++)
            {
                x = i / width;
                y = i % width;
                iMin = Mathf.Max(x - 1, 0);
                jMin = Mathf.Max(y - 1, 0);
                if (obstacle[x * width + y])
                    continue;
                for(int j = Mathf.Max(x - 1, 0); j <= Mathf.Min(x + 1, length - 1); j++)
                    for(int k = Mathf.Max(y - 1, 0); k <= Mathf.Min(y + 1, width - 1); k++)
                    {
                        if(integrationField[iMin * width + jMin] > integrationField[j * width + k])
                        {
                            iMin = j;
                            jMin = k;
                        }
                    }
                flowField[i] = new float3(iMin * gridLength - 500, 0, jMin * gridLength - 500);
                debug = Resources.Load("DebugArrow") as GameObject;
                debug.transform.position = new Vector3(iMin * gridLength - 500, 10, jMin * gridLength - 500);
                debug.transform.LookAt(new Vector3(start.x, 10, start.z));
                debug.SetActive(true);
                Instantiate(debug);
            }
            
        }

        integrationField.Dispose();
        generated = true;
    }

    void generateIntegrationField(float3 position)
    {
        List<int> openSet = new List<int>();
        resetField();

        int currentX = position.x / gridLength + 500 / gridLength;
        int currentY = position.z / gridLength + 500 / gridLength;

        if (currentX >= length || currentX < 0 || currentY >= width || currentY < 0)
            return;

        integrationField[currentX * width + currentY] = 0;

        openSet.Add(currentX * width + currentY);

        int currentID;

        List<int> neighbors;

        while (openSet.Count > 0)
        {
            currentID = openSet[0];
            openSet.Remove(currentID);

            neighbors = getNeighbors(currentID);
            

            for (int i = 0; i < neighbors.Count; i++)
            {
                
                
                if (integrationField[neighbors[i]] > (integrationField[currentID] + costField[neighbors[i]]))
                {
                    if (!openSet.Contains(neighbors[i]))
                        openSet.Add(neighbors[i]);
                    integrationField[neighbors[i]] = integrationField[currentID] + costField[neighbors[i]];
                    /*if (!obstacle[neighbors[i]] && integrationField[neighbors[i]] >= 65535)
                        integrationField[neighbors[i]] -= 65535;
                    else if (obstacle[neighbors[i]])
                        integrationField[neighbors[i]] = 65535;*/
                }
            }
        }
    }

    void TestCostField()
    {
        int x, z;
        for(int i = 0; i < costField.Length; i++)
        {
            x = i / width * gridLength - 500;
            z = i % width * gridLength - 500;
            GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameobj.transform.position = new Vector3(x, 10, z);
            if (costField[i] == 65535)
                gameobj.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        test = false;
    }

    void resetField()
    {
        for (int i = 0; i < length * width; i++)
            integrationField[i] = 65535;
    }

    List<int> getNeighbors(int currentID)
    {
        int x = currentID / width;
        int y = currentID % width;

        List<int> result = new List<int>();

        if (x + 1 <= length - 1 && costField[(x + 1) * width + y] < 65535)
            result.Add((x + 1) * width + y);
        if (x - 1 >= 0 && costField[(x - 1) * width + y] < 65535)
            result.Add((x - 1) * width + y);
        if (y + 1 <= width - 1 && costField[x * width + y + 1] < 65535)
            result.Add(x * width + y + 1);
        if (y - 1 >= 0 && costField[x * width + y - 1] < 65535)
            result.Add(x * width + y - 1);
        return result;
    }

    float3 Normalize(float3 position)
    {
        if (Mathf.Abs(position.x % gridLength) >= gridLength / 2)
            position.x = (position.x / gridLength + (position.x > 0 ? 1 : -1)) * gridLength;
        else
            position.x = position.x / gridLength * gridLength;
        if (Mathf.Abs(position.z % gridLength) >= gridLength / 2)
            position.z = (position.z / gridLength + (position.z > 0 ? 1 : -1)) * gridLength;
        else
            position.z = position.z / gridLength * gridLength;
        return position;
    }

    bool OutOfBound(float3 position)
    {
        if (position.x >= 500 || position.x <= -500 || position.z >= 500 || position.z <= -500)
            return true;
        return false;
    }

    public float3[] getFlowField()
    {
        generated = false;
        return flowField;
    }

    public bool hasGenerated()
    {
        return generated;
    }

    public static uint Vector3ToIndex(Vector3 pos)
    {
        uint result;
        uint x = (uint)(pos.x + 500);
        uint y = (uint)(pos.z + 500);
        if (x % gridLength >= gridLength / 2)
            x = x / (uint)gridLength + 1;
        else
            x = x / (uint)gridLength;
        if (y % gridLength >= gridLength / 2)
            y = y / (uint)gridLength + 1;
        else
            y = y / (uint)gridLength;
        result = (uint)(x * width + y);
        return result;
    }


    public static uint Float3ToIndex(float3 pos)
    {
        uint result;
        uint x = (uint)(pos.x + 500);
        uint y = (uint)(pos.z + 500);
        if (x % gridLength >= gridLength / 2)
            x = x / (uint)gridLength + 1;
        else
            x = x / (uint)gridLength;
        if (y % gridLength >= gridLength / 2)
            y = y / (uint)gridLength + 1;
        else
            y = y / (uint)gridLength;
        result = (uint)(x * width + y);
        return result;
    }

    public static uint Vector3ToIndex(Vector3 pos, int grid, int width)
    {
        uint result;
        uint x = (uint)(pos.x + 500);
        uint y = (uint)(pos.z + 500);
        if (x % grid >= grid / 2)
            x = x / (uint)grid + 1;
        else
            x = x / (uint)grid;
        if (y % grid >= grid / 2)
            y = y / (uint)grid + 1;
        else
            y = y / (uint)grid;
        result = (uint)(x * width + y);
        return result;
    }

    public static float3 IndexToFloat3(int index, int grid, int width)
    {
        int x = index / width * grid - 500;
        int z = index % width * grid - 500;
        int y = Mathf.RoundToInt(Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)));
        return new float3(x, y, z);
    }

    public static float3 Vector3ToFloat3(Vector3 input)
    {
        return new float3((int)input.x, (int)input.y, (int)input.z);
    }

    public static Vector3 Float3ToVector3(float3 input)
    {
        return new Vector3(input.x, input.y, input.z);
    }

    public static Vector3 IndexToVector3(int index, int grid)
    {
        int x = index / grid - 500;
        int z = index % grid - 500;
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
        return new Vector3(x, y, z);
    }
}
