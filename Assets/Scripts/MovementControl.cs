using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementControl : MonoBehaviour
{
    protected Animator animator;
    protected FlowField ff;
    protected float Turnspeed;
    protected private float Movingspeed;
    protected bool turning;
    protected bool moving;
    protected float angle;
    protected float distance;
    protected Vector3 target;
    protected Vector3 lastTarget;
    protected Vector3 finalTarget;
    protected int gridLength;
    protected List<Vector3> path;
    protected bool close;
    protected uint coordinate;
    protected float3[] flowfield;
    protected bool move;
    protected float time;
    protected bool disabled;
    protected float disableTime;
    protected float currentTime;
    protected float height;
    protected UnitLoad uLoad;
    protected bool atEnd;
    bool isidle;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        height = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHeight();
        transform.position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + height, transform.position.z);
        gridLength = GameObject.Find("EventSystem").GetComponent<FlowField>().getGridLength();
        animator = transform.GetChild(0).GetComponent<Animator>();
        ff = GameObject.Find("EventSystem").GetComponent<FlowField>();
        path = new List<Vector3>();
        flowfield = null;
        target = transform.position;
        lastTarget = target;
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

    protected virtual void FixedUpdate()
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

        Turn();

        if (move)
            Move();

        transform.GetChild(0).GetComponent<UnitLoad>().setPositionAndRotation(transform.gameObject);
    }

    protected virtual void Update()
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

    public void setDisabled(float duration)
    {
        disabled = true;
        disableTime = duration;
        currentTime = Time.time;
    }

    

    protected virtual void Move()
    {
        if (Mathf.Abs(distance) < 0.5f)
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

    protected virtual void Turn()
    {
        if (Mathf.Abs(angle) <= 0.2)
        {
            turning = false;
        }
        else if (angle < -0.2)
        {
            TurnLeft();
        }
        else if (angle > 0.2)
        {
            TurnRight();
        }
    }

    protected virtual void TurnLeft()
    {
        turning = true;
        if (angle + Turnspeed * Time.deltaTime > 0)
        {
            transform.eulerAngles -= new Vector3(0f, Mathf.Abs(angle), 0f);
            angle = 0;
            return;
        }
        transform.eulerAngles -= new Vector3(0f, Turnspeed * Time.deltaTime, 0f);
        angle += Turnspeed * Time.deltaTime;
    }
    protected virtual void TurnRight()
    {
        turning = true;
        if (angle - Turnspeed * Time.deltaTime < 0)
        {
            transform.eulerAngles += new Vector3(0f, angle, 0f);
            angle = 0;
            return;
        }
        transform.eulerAngles += new Vector3(0f, Turnspeed * Time.deltaTime, 0f);
        angle -= Turnspeed * Time.deltaTime;
    }

    public bool isIdle()
    {
        return Mathf.Abs(finalTarget.x - transform.position.x) <= 1 && Mathf.Abs(finalTarget.z - transform.position.z) <= 1;
    }

    public virtual void MoveTo(Vector3 target, bool canceltarget)
    {
        close = false;
        moving = false;
        turning = false;
        flowfield = null;
        path = new List<Vector3>();
        move = true;
        this.target = transform.position;
        distance = Mathf.Sqrt(Mathf.Pow(target.x - transform.position.x, 2) + Mathf.Pow(target.z - transform.position.z, 2));
        time = Time.time;
        //transform.GetChild(0).GetComponent<UnitLoad>().setAbilityActive(false);
        Attack attack;
        if (canceltarget && transform.GetChild(3).TryGetComponent<Attack>(out attack))
        {
            attack.stopAttack();
        }
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

    protected abstract void calculate();

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
        flowfield = null;
        path = new List<Vector3>();
        target = transform.position;
        finalTarget = transform.position;
    }

    public void addCoordinate(Vector3 coordinate)
    {
        if (path.Count != 0)
        {
            path.Add(target);
        }
        target = coordinate;
        moving = false;
        turning = false;
        calculate();
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

    protected void generatePath(Vector3 target)
    {
        ff.generate(FlowField.Vector3ToFloat3(target));
        float3[] field = ff.getFlowField();
        float3 NextPos = field[FlowField.Vector3ToIndex(transform.position)];
        path = new List<Vector3>();
        List<Vector3> Positions = new List<Vector3>();
        Positions.Add(FlowField.Float3ToVector3(NextPos));
        float3 ta = ff.getTarget();
        while (NextPos.x != ta.x || NextPos.z != ta.z)
        {
            NextPos = field[FlowField.Float3ToIndex(NextPos)];
            Positions.Add(FlowField.Float3ToVector3(NextPos));
        }

        if (Positions.Count > 3)
        {
            Pathfinder p = new Pathfinder(Positions[Positions.Count - 3], target, uLoad.OutputUnit().getSize(), true);
            Positions.RemoveRange(Positions.Count - 4, 4);
            path.AddRange(p.getPath());
            for (int i = Positions.Count - 1; i >= 0; i--)
                path.Add(Positions[i]);
        }
        else
        {
            Pathfinder p = new Pathfinder(transform.position, target, uLoad.OutputUnit().getSize(), true);
            path = p.getPath();
        }
    }

    private void OnDestroy()
    {
        if (GameObject.Find("Main Camera") == null)
            return;
        UnitSelection selection;
        if (GameObject.Find("Main Camera").TryGetComponent<UnitSelection>(out selection))
            selection.removeAt(gameObject);
    }
}
