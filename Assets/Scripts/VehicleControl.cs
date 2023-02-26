using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleControl : MonoBehaviour
{
    protected Animator animator;
    protected float Turnspeed;
    protected private float Movingspeed;
    protected bool turning;
    protected bool moving;
    protected float angle;
    protected float distance;
    protected float lastDistance;
    protected Vector3 target;
    protected Vector3 lastTarget;
    protected Vector3 finalTarget;
    protected int gridLength;
    protected List<Vector3> path;
    protected bool close;
    protected uint coordinate;
    protected float3[] FlowField;
    protected bool move;
    protected float time;
    protected bool disabled;
    protected float disableTime;
    protected float currentTime;
    protected float height;
    protected UnitLoad uLoad;
    protected bool atEnd;
    bool isidle;
    //GameObject RouteTrail;

    // Start is called before the first frame update
    void Start()
    {
        height = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHeight();
        transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + height, transform.position.z);
        gridLength = GameObject.Find("EventSystem").GetComponent<FlowField>().getGridLength();
        animator = transform.GetChild(0).GetComponent<Animator>();
        path = new List<Vector3>();
        target = transform.position;
        lastTarget = target;
        lastDistance = 1;
        distance = 0;
        finalTarget = transform.position;
        uLoad = transform.GetChild(0).GetComponent<UnitLoad>();
        Movingspeed = uLoad.OutputUnit().getSpeed();
        Turnspeed = uLoad.OutputUnit().getTurningSpeed();
        turning = false;
        moving = false;
        close = false;
        move = true;
        time = 0;
        disabled = false;
        disableTime = 0;
        atEnd = false;
        isidle = true;
    }

    private void Update()
    {
        if (disabled && Time.time - currentTime <= disableTime)
            return;
        else if (disabled)
        {
            disabled = false;
            disableTime = 0;
        }
        distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
        calculate();
    }

    void FixedUpdate()
    {
        isidle = isIdle();
        if (disabled && Time.time - currentTime <= disableTime)
            return;
        else if (disabled)
        {
            disabled = false;
            disableTime = 0;
        }
        Movingspeed = uLoad.OutputUnit().getSpeed();

        if (angle != 0)
            Turn();
        else
            turning = false;
        if (move)
            Move();

        transform.GetChild(0).GetComponent<UnitLoad>().setPositionAndRotation(transform.gameObject);

        if (Time.time - time > 3 && transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit() && lastDistance < distance)
        {
            turning = false;
            moving = false;
            close = true;
        }
        lastDistance = distance;
    }

    public void setDisabled(float duration)
    {
        disabled = true;
        disableTime = duration;
        currentTime = Time.time;
    }

    void Forward()
    {
        /*if(distance - Movingspeed * Time.deltaTime < 0)
        {
            transform.position += transform.forward * distance;
            distance = 0;
            return;
        }*/
        transform.position += transform.forward * Time.deltaTime * Movingspeed;
        //distance -= Movingspeed * Time.deltaTime;
    }

    void Backward()
    {
        /*if(distance + Movingspeed * Time.deltaTime > 0)
        {
            transform.position -= transform.forward * Mathf.Abs(distance);
            distance = 0;
            return;
        }*/
        transform.position -= transform.forward * Time.deltaTime * Movingspeed;
        //distance += Movingspeed * Time.deltaTime;
    }

    protected virtual void Move()
    {
        if (turning && (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isTrackUnit() || close))
            return;
        if(uLoad.OutputUnit().isFlyUnit())
        {
            if (Mathf.Abs(distance) < 1)
            {
                animator.SetBool("Forward", false);
                animator.SetBool("Backward", false);
                moving = false;
                close = false;
            }
            else if (distance >= 1)
            {
                animator.SetBool("Forward", true);
                moving = true;
                Forward();
            }
            else if (distance <= -1)
            {
                animator.SetBool("Backward", true);
                moving = true;
                Backward();
            }
            return;
        }
        if(Mathf.Abs(distance) < 0.5f)
        {
            animator.SetBool("Forward", false);
            moving = false;
            close = false;
        }
        else
        {
            moving = true;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, Terrain.activeTerrain.SampleHeight(target) + height, target.z), Movingspeed * Time.deltaTime);
        }
            
    }

    public bool isIdle()
    {
        return Mathf.Abs(finalTarget.x - transform.position.x) <= 1 && Mathf.Abs(finalTarget.z - transform.position.z) <= 1;
    }

    public void MoveTo(List<Vector3> route, float3 target) {
        GroundUnitCollision collision = gameObject.GetComponent<GroundUnitCollision>();
        if (!transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
        {
            collision.setDetect(true);
            collision.setClose(true);
            collision.setTarget(new Vector3(target.x, target.y, target.z));
        }
        transform.GetChild(0).GetComponent<UnitLoad>().setAbilityActive(false);
        transform.GetChild(3).GetComponent<Attack>().stopAttack();
        close = false;
        turning = false;
        moving = false;
        move = true;
        path = new List<Vector3>();
        atEnd = false;
        //FlowField = t;
        finalTarget = new Vector3(target.x, target.y, target.z);
        finalTarget.y = Terrain.activeTerrain.SampleHeight(finalTarget);
        this.target = transform.position;
        if (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
        {
            this.target = finalTarget;
            lastTarget = finalTarget;
        }
        else
        {
            //this.gridLength = gridLength;
            //coordinate = getCoordinate(transform.position, t.Length / (1000 / gridLength));
            //this.target = new Vector3(t[coordinate].x, t[coordinate].y, t[coordinate].z);
            lastTarget = finalTarget;
            //collision.setCount(GameObject.Find("Main Camera").GetComponent<UnitSelection>().getCurrentSelected().Count);
            path = route;
        }
        Vector3 next = path[path.Count - 1];
        path.RemoveAt(path.Count - 1);
        distance = Mathf.Sqrt(Mathf.Pow(next.x - transform.position.x, 2) + Mathf.Pow(next.z - transform.position.z, 2));
        lastDistance = distance + 1;
        time = Time.time;
        if (distance < 20)
        {
            if(transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                close = true;
            turning = false;
            moving = false;
        }
        Quaternion rotation = Quaternion.LookRotation(next - transform.position);
        angle = rotation.eulerAngles.y - transform.eulerAngles.y;
        if (angle > 185)
            angle -= 360;
        if (angle < -185)
            angle += 360;
    }

    public void MoveTo(Vector3 target, bool considerUnit)
    {
        close = false;
        moving = false;
        turning = false;
        FlowField = null;
        move = true;
        this.target = transform.position;
        if (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isTrackUnit())
        {
            gameObject.GetComponent<GroundUnitCollision>().setDetect(false);
            gameObject.GetComponent<GroundUnitCollision>().setClose(false);
        }
        if (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
        {
            finalTarget = target;
            finalTarget.y = Terrain.activeTerrain.SampleHeight(finalTarget);
            this.target = finalTarget;
            lastTarget = finalTarget;
        }
        else
        {
            Pathfinder pathfinding = new Pathfinder(transform.position, target, 10, considerUnit);
            path = pathfinding.getPath();
            //foreach(Vector3 pos in path)
            //{
            //    GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    gameobj.GetComponent<BoxCollider>().enabled = false;
            //    gameobj.transform.position = pos + new Vector3(0, 2, 0);
            //    gameobj.SetActive(true);
            //}
            finalTarget = pathfinding.getTarget();
            target = path[path.Count - 1];
            path.Remove(target);
            lastTarget = finalTarget;
        }
        distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
        lastDistance = distance + 1;
        time = Time.time;
        if (distance < 20)
        {
            if (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                close = true;
            turning = false;
            moving = false;
        }
        Quaternion rotation = Quaternion.LookRotation(target - transform.position);
        angle = rotation.eulerAngles.y - transform.eulerAngles.y;
        if (angle > 185)
            angle -= 360;
        if (angle < -185)
            angle += 360;
    }

    uint getCoordinate(Vector3 position, int width)
    {
        uint result;
        uint x = (uint)(position.x + 500);
        uint y = (uint)(position.z + 500);
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

    public void setAngle(GameObject gameobj)
    {
        Quaternion rotation = Quaternion.LookRotation(gameobj.transform.position - transform.position);
        angle = rotation.eulerAngles.y - transform.eulerAngles.y;
    }

    void calculate()
    {
        if ((turning || moving) && (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isTrackUnit() || close))
        {
            return;
        }
        if (Mathf.Abs(target.x - transform.position.x) > 1 || Mathf.Abs(target.z - transform.position.z) > 1)
        {
            Quaternion rotation = Quaternion.LookRotation(target - transform.position);
            angle = rotation.eulerAngles.y - transform.eulerAngles.y;
            if (angle > 185)
                angle -= 360;
            if (angle < -185)
                angle += 360;
            distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
        }
        else
        {
            if(transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                target = transform.position;
            else
            {
                if(Mathf.Abs(transform.position.x - finalTarget.x) <= 1 && Mathf.Abs(transform.position.z - finalTarget.z) <= 1)
                {
                    finalTarget = transform.position;
                    target = transform.position;
                }
                if (path.Count > 0)
                {
                    target = path[path.Count - 1];
                    target.y = Terrain.activeTerrain.SampleHeight(target);
                    path.Remove(path[path.Count - 1]);
                }
                else if(Mathf.Abs(transform.position.x - finalTarget.x) > 1 || Mathf.Abs(transform.position.z - finalTarget.z) > 1)
                {
                    target = finalTarget;
                    target.y = Terrain.activeTerrain.SampleHeight(target);
                }
                else
                {
                    finalTarget = transform.position;
                    target = transform.position;
                    target.y = Terrain.activeTerrain.SampleHeight(target);
                }
            }

        }
            
    }

    protected virtual void Turn() {
        if (moving && (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isTrackUnit() || close))
            return;
        if (transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isInfantry())
        {
            transform.LookAt(target);
            transform.eulerAngles -= new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            angle = 0;
            turning = false;
            return;
        }
        if (Mathf.Abs(angle) > 0.2)
        {
            if(!transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                animator.SetBool("Forward", true);
            turning = true;
        }
        if (Mathf.Abs(angle) <= 0.2)
        {
            if(!transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                animator.SetBool("Forward", false);
            turning = false;
        }
        /*else
        {
            Vector3 temp = new Vector3(target.x, transform.position.y, target.z);
            Quaternion rotation = Quaternion.LookRotation(temp - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Turnspeed);
        }*/
        else if (angle < -0.2)
        {
            TurnLeft();
        }
        else if(angle > 0.2)
        {
            TurnRight();
        }
    }

    void TurnLeft()
    {
        if(angle + Turnspeed * Time.deltaTime > 0)
        {
            transform.eulerAngles -= new Vector3(0f, Mathf.Abs(angle), 0f);
            angle = 0;
            return;
        }
        transform.eulerAngles -= new Vector3(0f, Turnspeed * Time.deltaTime, 0f);
        angle += Turnspeed * Time.deltaTime;
    }
    void TurnRight()
    {
        if(angle - Turnspeed * Time.deltaTime < 0)
        {
            transform.eulerAngles += new Vector3(0f, angle, 0f);
            angle = 0;
            return;
        }
        transform.eulerAngles += new Vector3(0f, Turnspeed * Time.deltaTime, 0f);
        angle -= Turnspeed * Time.deltaTime;
    }

    public bool isMoving()
    {
        return moving;
    }

    public bool isTurning()
    {
        return turning;
    }

    public void setMove(bool move)
    {
        this.move = move;
    }

    public bool getMove()
    {
        return move;
    }

    public Vector3 getTarget()
    {
        return finalTarget;
    }

    public Vector3 getCurrentTarget()
    {
        return target;
    }

    public Vector3 getLastTarget()
    {
        return lastTarget;
    }

    public void cancelTarget()
    {
        moving = false;
        turning = false;
        close = false;
        angle = 0f;
        distance = 0f;
        FlowField = null;
        path = new List<Vector3>();
        target = transform.position;
        finalTarget = transform.position;
    }

    private void OnDestroy()
    {
        if (GameObject.Find("Main Camera") == null)
            return;
        UnitSelection selection;
        if(GameObject.Find("Main Camera").TryGetComponent<UnitSelection>(out selection))
           selection.removeAt(gameObject);
    }

    public void addCoordinate(Vector3 coordinate)
    {
        if(path.Count != 0)
        {
            path.Add(target);
        }
        target = coordinate;
        moving = false;
        turning = false;
        calculate();
    }

    public float3[] getFlowField()
    {
        return FlowField;
    }

    public void setCurrentTarget(Vector3 t)
    {
        close = false;
        moving = false;
        turning = false;
        target = t;
    }

    public List<Vector3> getPath()
    {
        return path;
    }
}
