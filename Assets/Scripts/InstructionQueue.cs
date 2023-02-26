using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionQueue : MonoBehaviour
{
    LinkedList<Instruction> InsQueue;
    FlowField ff;
    MovementControl control;
    Attack attack;
    UnitLoad uLoad;
    Instruction next;
    int main;
    bool finished;
    int count;
    int currentType;
    bool active;
    bool a_move;
    GameObject RouteTrail;
    List<Vector3> path;

    bool isMain;

    //Instruction Table
    //0: Go to Target without cancel attack target
    //1: Attack Target unit
    //2: Go inside target unit
    //3: Go to Target and cancel attack target
    //4: Cancel Movement
    //5: A move to target
    //6: Skip current Instruction


    // Start is called before the first frame update
    void Awake()
    {
        InsQueue = new LinkedList<Instruction>();
        path = new List<Vector3>();
        count = 0;
        gameObject.TryGetComponent<MovementControl>(out control);
        transform.GetChild(3).TryGetComponent<Attack>(out attack);
        uLoad = transform.GetChild(0).GetComponent<UnitLoad>();
        ff = GameObject.Find("EventSystem").GetComponent<FlowField>();
        finished = true;
        currentType = -1;
        active = true;
        a_move = false;
        RouteTrail = null;
        main = -1;
        isMain = false;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (RouteTrail != null && main != -1)
            {
                List<Vector3> points = new List<Vector3>();
                if (main == 1)
                {
                    RouteTrail.GetComponent<LineRenderer>().positionCount = control.getPath().Count + 2;
                    points.Add(new Vector3(next.getTarget().transform.position.x, 0, next.getTarget().transform.position.z));
                    points.AddRange(control.getPath());
                    if(!control.isIdle())
                    {
                        RouteTrail.GetComponent<LineRenderer>().positionCount++;
                        Vector3 ct = control.getTarget();
                        ct.y = 0;
                        points.Add(ct);
                    }
                    points.Add(new Vector3(transform.position.x, 0, transform.position.z));
                }
                else/* if(next.getInstructionType() == 0 || next.getInstructionType() == 2)*/
                {
                    RouteTrail.GetComponent<LineRenderer>().positionCount = control.getPath().Count + 2;
                    points.AddRange(control.getPath());
                    Vector3 ct = control.getCurrentTarget();
                    ct.y = 0;
                    points.Add(ct);
                    points.Add(new Vector3(transform.position.x, 0, transform.position.z));
                }
                RouteTrail.GetComponent<LineRenderer>().SetPositions(points.ToArray());
            }
        }
        catch (MissingReferenceException) { Destroy(RouteTrail); }
        catch (System.NullReferenceException) { Destroy(RouteTrail); }
        if (!active)
            return;
        count = InsQueue.Count;
        if(next != null && next.getInstructionType() == 2)
        {
            Vector3 pos = transform.position, target = next.getTarget().transform.position;
            if (Mathf.Abs(target.x - pos.x) <= 10 && Mathf.Abs(target.z - pos.z) <= 10)
            {
                next.getTarget().transform.GetChild(0).GetComponent<UnitLoad>().addTroop(gameObject);
                nextInstruction();
                return;
            }
        }
        if(next != null && next.getInstructionType() == 5 && a_move)
        {
            //Debug.Log((attack.getTarget() == null) + " " + attack.getTargetList().Count);
            if (attack.getTarget() != null)
                addAndStash(new Instruction(1, attack.getTarget()));
            else if (attack.getTargetList().Count > 0)
            {
                /*List<GameObject> al = attack.getTargetList();
                if (al[0] == null)
                    attack.RemoveTarget(null);
                else
                    addAndStash(new Instruction(1, al[0]));*/
                addAndStash(new Instruction(1, attack.NextTarget()));
            }
        }
        finished = InsComplete();
        if (finished)
            nextInstruction();
    }

    public void nextInstruction()
    {

        if (Count() == 0)
        {
            next = null;
            main = -1;
            return;
        }

        /*if(transform.GetChild(0).name.Equals("ArmedHelicopter"))
        {
            Debug.Log((new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            Debug.Log("next Instruction");
        }*/

        //control.cancelTarget();
        //attack.stopAttack();

        //Debug.Log("Count: " + InsQueue.Count);
        next = InsQueue.First.Value;
        //Debug.Log("Type: " + next.getInstructionType());

        InsQueue.RemoveFirst();
        if (a_move)
            a_move = false;
        switch (next.getInstructionType())
        {
            case 0:
                control.MoveTo(next.getPosition(), false);
                
                if(!uLoad.OutputUnit().isStatic() && control != null)
                {
                    if (RouteTrail != null)
                        Destroy(RouteTrail);
                    main = next.getInstructionType();
                    this.path = control.getPath();
                    RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                        RouteTrail.SetActive(false);
                }

                break;
            case 1:
                if (next.getTarget() == null)
                    Debug.Log("Target is null!");
                if(control != null)
                    control.cancelTarget();
                attack.stopAttack();
                if(next.getTarget() == null)
                {
                    finished = true;
                    return;
                }
                attack.setTarget(next.getTarget());
                
                if (RouteTrail != null)
                    Destroy(RouteTrail);
                
                main = next.getInstructionType();

                if(!uLoad.OutputUnit().isStatic())
                {
                    if (RouteTrail != null)
                        Destroy(RouteTrail);
                    this.path = control.getPath();
                    RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                        RouteTrail.SetActive(false);
                }
                
                break;
            case 2:
                GameObject target = next.getTarget();
                Vector3 pos2 = target.transform.position;
                float3 t2 = new float3((int)pos2.x, (int)pos2.y, (int)pos2.z);
                /*if (uLoad.OutputUnit().isFlyUnit())
                {
                    control.MoveTo(pos2, false);
                    if (!next.isSubTask())
                    {
                        if (RouteTrail != null)
                            Destroy(RouteTrail);
                        main = next.getInstructionType();
                        this.path = control.getPath();
                        RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                        if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                            RouteTrail.SetActive(false);
                    }
                    return;
                }
                ff.generate(t2);
                List<Vector3> Path = GeneratePath(ff.getFlowField(), target.transform.position);
                control.MoveTo(Path, t2);*/
                control.MoveTo(pos2, true);

                if(!uLoad.OutputUnit().isStatic() && control == null)
                {
                    if (RouteTrail != null)
                        Destroy(RouteTrail);
                    main = next.getInstructionType();
                    this.path = control.getPath();
                    RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                        RouteTrail.SetActive(false);
                }

                break;
            case 3:
                float3 t = new float3((int)next.getPosition().x, (int)next.getPosition().y, (int)next.getPosition().z);
                /*if (gameObject.GetComponent<MovementControl>() != null)
                {
                    gameObject.GetComponent<MovementControl>().MoveTo(next.getPosition(), false);
                    return;
                }*/
                //List<Vector3> Positions = GeneratePath(next.getFlowField(), next.getPosition());
                if (control == null)
                {
                    nextInstruction();
                    return;
                }
                control.MoveTo(next.getPosition(), true);

                if(!uLoad.OutputUnit().isStatic() && control != null)
                {
                    if (RouteTrail != null)
                        Destroy(RouteTrail);
                    main = next.getInstructionType();
                    this.path = control.getPath();
                    RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                        RouteTrail.SetActive(false);
                }

                break;
            case 4:
                control.cancelTarget();
                break;
            case 5:
                /*float3 t5 = new float3((int)next.getPosition().x, (int)next.getPosition().y, (int)next.getPosition().z);
                float3[] ffield;
                if (next.getFlowField() == null)
                {
                    ff.generate(t5);
                    ffield = ff.getFlowField();
                }
                else
                    ffield = next.getFlowField();
                List<Vector3> path = GeneratePath(ffield, new Vector3(t5.x, t5.y, t5.z));*/
                control.MoveTo(next.getPosition(), true);

                if(!uLoad.OutputUnit().isStatic() && control != null)
                {
                    if (RouteTrail != null)
                        Destroy(RouteTrail);
                    main = next.getInstructionType();
                    this.path = control.getPath();
                    RouteTrail = Instantiate(Resources.Load("RouteTrail") as GameObject, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    if (!transform.GetChild(1).GetChild(0).gameObject.activeSelf)
                        RouteTrail.SetActive(false);
                }

                a_move = true;
                break;
            case 6:
                nextInstruction();
                break;
        }
        currentType = next.getInstructionType();
        //if (transform.GetChild(0).name.Equals("APC"))
        //Debug.Log("next Instruction: " + next.getInstructionType());
        finished = false;
    }

    bool InsComplete()
    {
        if (next == null || finished)
            return true;
        if(next.getInstructionType() == 0 || next.getInstructionType() == 3 || next.getInstructionType() == 5)
        {
            if (uLoad.OutputUnit().isStatic())
                return true;
            if (control.isIdle())
            {
                if (a_move)
                    a_move = false;
                return true;
            }
            else
                return false;
        }
        if(next.getInstructionType() == 1)
        {
            if ((uLoad.OutputUnit().isStatic() && attack.getTarget() == null) || next.getTarget() == null || next.getTarget().transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() <= 0)
            {
                FighterAttack fa = transform.GetChild(3).GetComponent<FighterAttack>();

                if (fa != null && control.isIdle())
                    return true;
                else if (fa == null)
                {
                    attack.RemoveTarget(null);
                    if (next.getTarget() != null)
                    {
                        attack.RemoveTarget(next.getTarget());
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        if(next.getInstructionType() == 2)
        {
            if (next.getTarget().transform.GetChild(0).GetComponent<UnitLoad>().troopInVehicle(gameObject))
                return true;
            else
                return false;
        }
        if (next.getInstructionType() == 4 || next.getInstructionType() == 6)
            return true;
        return false;
    }

    List<Vector3> GeneratePath(float3[] field, Vector3 final)
    {
        float3 NextPos = field[FlowField.Vector3ToIndex(transform.position)];
        List<Vector3> Positions = new List<Vector3>();
        Positions.Add(new Vector3(NextPos.x, NextPos.y, NextPos.z));
        float3 ta = ff.getTarget();
        List<Vector3> result = new List<Vector3>();
        //Pathfinder pathfinder = new Pathfinder(transform.position, final, uLoad.OutputUnit().getSize(), false);
        //result = pathfinder.getPath();
        while (NextPos.x != ta.x || NextPos.z != ta.z)
        {
            NextPos = field[FlowField.Float3ToIndex(NextPos)];
            Positions.Add(new Vector3(NextPos.x, NextPos.y, NextPos.z));
        }
        
        if (final == null)
        {
            for (int i = Positions.Count - 1; i >= 0; i--)
                result.Add(Positions[i]);
            return result;
        }

        if (Positions.Count > 5)
        {
            Pathfinder p = new Pathfinder(Positions[Positions.Count - 6], final, uLoad.OutputUnit().getSize(), false);
            Positions.RemoveRange(Positions.Count - 6, 6);
            result.AddRange(p.getPath());
            for (int i = Positions.Count - 1; i >= 0; i--)
                result.Add(Positions[i]);
        }
        else
        {
            Pathfinder p = new Pathfinder(transform.position, final, uLoad.OutputUnit().getSize(), false);
            result = p.getPath();
        }
        return result;
    }

    public void addInstruction(Instruction ins)
    {
        if (!active)
            return;
        if (ins.getInstructionType() == 0 && control == null)
            return;
        if (ins.getInstructionType() == 1 && attack == null)
            return;
        InsQueue.AddLast(ins);
        if (ins.getInstructionType() == 0 && transform.GetChild(0).name.Equals("LeviTank"))
            Debug.Log("add instruction " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
    }

    public void addInstructionToFront(Instruction ins)
    {
        if (!active)
            return;
        if (ins.getInstructionType() == 0 && control == null)
            return;
        if (ins.getInstructionType() == 1 && attack == null)
            return;
        InsQueue.AddFirst(ins);
        if (ins.getInstructionType() == 0 && transform.GetChild(0).name.Equals("LeviTank"))
            Debug.Log("add instruction to front " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        nextInstruction();
    }

    public void addAndStash(Instruction ins)
    {
        if(next != null)
            InsQueue.AddFirst(next);
        InsQueue.AddFirst(ins);
        next = null;
        //if (ins.getInstructionType() == 0 && transform.GetChild(0).name.Equals("LeviTank"))
          //  Debug.Log("add and stash " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        nextInstruction();
    }    

    public void clearAndExecute(Instruction ins)
    {
        clearQueue();
        addInstruction(ins);
        if (ins.getInstructionType() == 0 && transform.GetChild(0).name.Equals("LeviTank"))
            Debug.Log("clear and execute " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
        nextInstruction();
    }

    public void clearQueue()
    {
        if (control != null)
            control.cancelTarget();
        if (attack != null)
            attack.stopAttack();
        //Debug.Log("Clear Queue");
        InsQueue.Clear();
        finished = true;
        next = null;
        Destroy(RouteTrail);
    }

    public int Count()
    {
        return InsQueue.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (next == null || (other.gameObject.layer != 3 && other.gameObject.layer != 8))
            return;
        //if (next.getInstructionType() == 2)
          //  Debug.Log(other.name);
        if (next.getInstructionType() == 2 && next.getTarget() == other.gameObject)
        {
            other.transform.GetChild(0).GetComponent<UnitLoad>().addTroop(gameObject);
            nextInstruction();
        }
    }

    public bool isIdle()
    {
        return finished && (InsQueue.Count == 0);
    }    

    public void setActive(bool active)
    {
        this.active = active;
    }

    public void StopMoving()
    {
        if (control == null)
            return;
        control.cancelTarget();
        if (next.getInstructionType() == 0 || next.getInstructionType() == 3)
            nextInstruction();
    }

    public void StopAttacking()
    {
        if (attack == null)
            return;
        attack.stopAttack();
        if (next.getInstructionType() == 1)
            nextInstruction();
    }

    public void Destroy()
    {
        if (RouteTrail != null)
            Destroy(RouteTrail);
    }

    public bool isAMove()
    {
        return a_move;
    }

    public int getCurrentType()
    {
        return currentType;
    }

    public void SetRouteActive(bool active)
    {
        if(RouteTrail != null)
            RouteTrail.SetActive(active);
    }

    private void OnDestroy()
    {
        if (RouteTrail != null)
            Destroy(RouteTrail);
    }
}
