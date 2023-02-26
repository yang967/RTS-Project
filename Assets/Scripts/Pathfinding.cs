using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    List<Spot> openSet;     //store discovered but unprocessed positions
    List<Spot> closeSet;    //store processed positions
    HashSet<Vector3> close;
    bool[] explored;
    List<Vector3> correctPath;  //store path from start to end
    GameObject startObject; //GameObject that will follow the correctPath
    bool considerUnit;  //If true, then the algorithm will consider objects in unit layer as obstacle
    Ray ray;

    Spot start;     //start spot
    Vector3 startVector;    //start position
    Vector3 end;    //end position

    int num;//debug variable
    float inserttime;
    float findtime;
    //definition of f, g, h is in Spot class

    /**
     * Main Constructor
     * @param start_position, end_position, GameObject_Transform_information, isConsiderUnit
     */
    public Pathfinding(Vector3 start, Vector3 end, Transform transform, bool ConsiderUnit)
    {
        //float t = Time.realtimeSinceStartup;
        explored = new bool[200 * 200];
        startObject = transform.gameObject;
        considerUnit = ConsiderUnit;
        startVector = start;
        inserttime = 0;
        findtime = 0;
        //float time = Time.realtimeSinceStartup;
        //--------Output last if statement time usage
        //Debug.Log("search 1: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        //time = Time.realtimeSinceStartup;
        //----------------------------------
        Vector3 center = end;
        int i = 10;     //i as radius
        bool renewed;
        if (isContain(end))
            renewed = false;
        else
            renewed = true;
        while(i < 500 && !renewed)      //if radius >= 500 or has found an nearest reachable position, then end the loop
        {
                
            for (int angle = (int)transform.eulerAngles.y; angle <= 360; angle += 30)   // angle start with current heading direction, ends at 360 degree, increased by 30 degree everytime
            {
                end = new Vector3(Mathf.Cos(Mathf.Rad2Deg * angle) * i + center.x, center.y, Mathf.Sin(Mathf.Rad2Deg * angle) * i + center.z);  //using angle and radius to calculate the position
                //--------Mark the current position
                //GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //gameobj.transform.position = end;
                //gameobj.GetComponent<Renderer>().material.color = Color.blue;
                //gameobj.SetActive(true);
                //----------------------------------
                if (!isContain(end))    //if the current position has no obstacles nearby, then end the loop
                {
                    renewed = true;
                    break;
                }
            }
            i += 10;    //radius increased by 10
        }
        //--------Output last if statement time usage
        //Debug.Log("search 2: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        //time = Time.realtimeSinceStartup;
        //----------------------------------
        //--------Mark the end position
        //GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //gameobj.transform.position = end + new Vector3(0, 2, 0);
        //gameobj.SetActive(true);
        //----------------------------------
        this.start = new Spot(start, end);  //set start spot
        this.end = end;
        openSet = new List<Spot>();
        closeSet = new List<Spot>();
        close = new HashSet<Vector3>();
        float time = Time.realtimeSinceStartup;
        DivideNConquer.Insert(openSet, this.start, 0, openSet.Count - 1);
        //openSet.Add(this.start);    //add start spot to unprocessed list
        inserttime += Time.realtimeSinceStartup - time;
        
        correctPath = new List<Vector3>();
        num = 0;
        //--------Output initialization time usage
        //Debug.Log("intialize: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        //time = Time.realtimeSinceStartup;
        //----------------------------------
        CalculatePath();    //calculate correct path
        //Debug.Log("Find time: " + findtime / 1000 + "ms");
        //Debug.Log("Insert time: " + inserttime / 1000 + "ms");
        //--------Output calculating correct path time usage
        //Debug.Log("Calculate: " + (Time.realtimeSinceStartup - time) * 1000 + "ms");
        //----------------------------------
        //Debug.Log((Time.realtimeSinceStartup - t) * 1000 + "ms");
    }

    /**
     * Calculate path from start to end
     */
    private void CalculatePath()
    {
        Spot current = Min();   //current as current_Spot
        //current_spot initialized as spot with smallest (distance_to_endpoint_ignore_obstacle + distance_to_startpoint_ignore_obstacle) in openSet
        int i = 0;  //debug variable
        float tt;
        while(current.h != 0 && openSet.Count != 0)
        {
            getSurrounding(current);    //add all reachable spot near current_spot to openSet
            
            openSet.Remove(current);    //remove current_spot from openSet
            tt = Time.realtimeSinceStartup;
            DivideNConquer.Insert(closeSet, current, 0, closeSet.Count - 1);
            close.Add(current.current);
            //closeSet.Add(current);  //add current_Spot to closeSet (this spot will not be processed again)
            inserttime += Time.realtimeSinceStartup + tt;
            
            current = Min();    //get spot for next loop
            i++;
            if (i > 25000)     //if i over 250000, then considered as Stack Overflow
            {
                Debug.Log("Stack Overflow!");
                current = null;
            }
            if (current == null)    //if cannot find next spot or StackOverflow, then exit loop
                break;
        }
        if (current != null)    //if success to reach the end_position, then get the correct path from start_position to end_position
            getCorrectPath(current);
        else
            getCorrectPath(closestPoint()); //if fail to reach the end_position, then get correct path from start_position to the closest point of the end_position
    }

    Vector3 IntVector3(Vector3 toInt)
    {
        return new Vector3((int)toInt.x, (int)toInt.y, (int)toInt.z);
    }

    public List<Vector3> getPath()
    {
        return correctPath;
    }

    /**
     * add all reachable Spot near input spot to openSet
     * @param current_Spot
     */
    private void getSurrounding(Spot current)
    {
        //ignore decimal of Spot's x and y coordinate (in Unity3d, y coordinate stored in Vector3.z)
        int x = (int)current.current.x;
        int y = (int)current.current.z;
        Vector3 pos;    //pos as current_position
        int k;
        float time;
        bool contain;
        for(int i = x - 5; i <= x + 5; i += 5)
        {
            for(int j = y - 5; j <= y + 5; j += 5)
            {
                pos = new Vector3(i, 0f, j);
                pos.y = Terrain.activeTerrain.SampleHeight(pos) + 0.5f; //correct the height of the current_position
                time = Time.realtimeSinceStartup;
                contain = close.Contains(pos);
                findtime += Time.realtimeSinceStartup - time;
                //k = exist(openSet, pos);    //check if current_position exists in openSet
                k = DivideNConquer.Find(openSet, pos, 0, openSet.Count - 1);
                //n = exist(closeSet, pos);   //check if current_position exists in closeSet
                //n = DivideNConquer.Find(closeSet, pos, 0, closeSet.Count - 1);

                //findtime += Time.realtimeSinceStartup - time;
                //--------Mark the current position
                //GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //gameobj.transform.position = pos;
                //gameobj.SetActive(true);
                //----------------------------------
                num++;
                
                //if current_position exists in closeSet or openSet or currentPosition has obstacle nearby or current_Position is not in map, then ignore current_Position
                if (k != -1 || isContain(pos) || outofBound(pos) || contain)
                {
                    //if current_position exists in openSet, then change that Spot's precede Spot and its f and g value
                    if (k != -1)
                    {
                        openSet[k].setPrecede(current);
                    }
                    //--------Mark unreachable position
                    //gameobj.GetComponent<Renderer>().material.color = Color.red;
                    //----------------------------------
                    //--------Destroy duplicate position marker
                    //if (k != -1 || n != -1)
                    //    MonoBehaviour.Destroy(gameobj);
                    //----------------------------------
                    continue;
                }
                time = Time.realtimeSinceStartup;
                DivideNConquer.Insert(openSet, new Spot(end, pos, current, start), 0, openSet.Count - 1);
                //openSet.Add(new Spot(end, pos, current, start));    //add current_position to openSet
                inserttime += Time.realtimeSinceStartup - time;
                
            }
        }
    }

    /**
     * return the Spot with smallest h value in closeSet
     */
    private Spot closestPoint()
    {
        Spot closest = closeSet[0];
        for(int i = 1; i < closeSet.Count; i++)
        {
            if (closeSet[i].h < closest.h)
                closest = closeSet[i];
            else if (closeSet[i].h == closest.h)
                if (closeSet[i].f < closest.f)
                    closest = closeSet[i];
        }
        return closest;
    }

    private void getCorrectPath(Spot current)
    {
        //GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //gameobj.transform.position = current.current + new Vector3(0, 5, 0);
        correctPath.Add(current.current);
        if (current.precede == null)
            return;
        getCorrectPath(current.precede);
    }

    private bool outofBound(Vector3 toCheck)
    {
        if (toCheck.x > 500 || toCheck.x < -500 || toCheck.z > 500 || toCheck.z < -500)
            return true;
        return false;
    }

    private int exist(List<Spot> set, Vector3 toFind)
    {
        for(int i = 0; i < set.Count; i++)
        {
            if ((int)set[i].current.x == (int)toFind.x && (int)set[i].current.z == (int)toFind.z)
                return i;
        }
        return -1;
    }

    /**
     * return the Spot with smallest f value in openSet
     */
    private Spot Min()
    {
        if (openSet.Count == 0)
            return null;
        Spot min = openSet[0];
        for(int i = 1; i < openSet.Count; i++)
        {
            if(openSet[i].f < min.f)
            {
                min = openSet[i];
            }
            else if(openSet[i].f == min.f)
            {
                if (openSet[i].h < min.h)
                    min = openSet[i];
            }
        }
        return min;
    }

    public bool isReachable()
    {
        return openSet.Count == 0;
    }

    public Vector3 getTarget()
    {
        return end;
    }

    /**
     * check if the input position has obstacles nearby
     * @param current_position
     */
    private bool isContain(Vector3 point)
    {
        RaycastHit rayhit = new RaycastHit();
        try
        {
            ray.origin = point + new Vector3(0, Mathf.Min(2, point.y), 0);
            ray.direction = Vector3.up;
            if (Physics.Raycast(ray, out rayhit, 1))
                //if (considerUnit? point == startVector : (rayhit.transform.gameObject.layer != 3 || point == startVector))
                if(/*point == startVector || */considerUnit? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = Vector3.forward;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;

            ray.direction = Vector3.back;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = Vector3.right;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = Vector3.left;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = (Vector3.forward + Vector3.right).normalized;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = (Vector3.forward + Vector3.left).normalized;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = (Vector3.back + Vector3.right).normalized;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
            ray.direction = (Vector3.back + Vector3.left).normalized;
            if (Physics.Raycast(ray, out rayhit, 20))
                if (/*point == startVector || */considerUnit ? !rayhit.transform.name.Equals(startObject.name) : rayhit.transform.gameObject.layer != 3)
                    return true;
        }
        catch(System.Exception ex)
        {
            Debug.Log(ex);
        }
        return false;
    }
}

public class Spot
{
    public int g;   //distance between current position and start position, ignore obstacle
    public int h;   //distance between current poisition and end position, ignore obstacle
    public int f;   //sum of g and h
    public Spot precede;    //the position that can lead to current position
    public Vector3 current;     //current position

    /**
     * Default constructor
     */
    public Spot()
    {
        g = 0;
        h = int.MaxValue;
        f = int.MaxValue;
    }

    /**
     * Constructor for start point
     * @param start_position, end_position
     */
    public Spot(Vector3 start, Vector3 end)
    {
        current = new Vector3(start.x, start.y, start.z);
        precede = null;
        g = 0;
        h = getDistance(start, end);
        f = g + h;
    }

    /**
     * Constructor for non start point
     * @param end_position, current_position, precede_Spot, start_Spot
     */
    public Spot(Vector3 end, Vector3 current, Spot precede, Spot start)
    {
        this.precede = precede;
        this.current = current;
        h = getDistance(current, end);
        g = getDistance(start.current, current);
        f = g + h;
    }


    /**
     * set the precede as the input precede and re-calculate the current f and g value
     * @param precede_Spot
     */
    public void setPrecede(Spot precede)
    {
        if (this.precede == null)
            return;
        if(this.precede.g > precede.g)
        {
            g = precede.g + getDistance(precede.current, current);
            f = g + h;
        }
    }

    public int Compare(Spot other)
    {
        if (f == other.f && h == other.h)
            return 0;
        if (f < other.f || (f == other.f && h < other.h))
            return -1;
        else
            return 1;
    }

    /**
     * Calculate distance between positions
     * @param start_position, end_position
     */
    private int getDistance(Vector3 start, Vector3 end)
    {
        int distance;
        int x = (int)Mathf.Abs(end.x - start.x) / 5;
        int y = (int)Mathf.Abs(end.z - start.z) / 5;
        distance = (Mathf.Max(x, y) - Mathf.Min(x, y)) * 2 + Mathf.Min(x, y) * 3;
        return distance;
    }

}
