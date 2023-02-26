using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour
{
    public Camera cam;
    [SerializeField] private GameObject waypoint;
    Vector3 mouseDown;
    Vector3 target;
    bool active;
    int group;
    List<GameObject> selected;
    GameObject AttackedObject;
    FlowField ff;
    bool a_move;
    UnitSelection selection;

    void Start()
    {
        selection = GameObject.Find("Main Camera").GetComponent<UnitSelection>();
        group = GameObject.Find("Main Camera").GetComponent<CameraControl>().getGroup();
        selected = new List<GameObject>();
        waypoint.SetActive(false);
        AttackedObject = null;
        active = true;
        ff = GameObject.Find("EventSystem").GetComponent<FlowField>();
        a_move = false;
    }
    
    void Update()
    {
        if (!active)
            return;
        selected = selection.getCurrentSelected();
        //if(Input.GetMouseButtonUp(0) && a_move)

        if (Input.GetKeyUp(KeyCode.A) && selected.Count > 0)
            a_move = true;
        if (a_move && Input.GetMouseButtonUp(1))
            a_move = false;

        if (Input.GetMouseButtonDown(1))
            mouseDown = Input.mousePosition;
        if (Input.GetMouseButtonUp(1) || (Input.GetMouseButtonUp(0) && a_move))
        {
            if (!click())
            {
                waypoint.SetActive(false);
            }
            else
            {
                ff.generate(new float3((int)transform.position.x, (int)transform.position.y, (int)transform.position.z));
                setTarget(ff.getFlowField(), Input.GetKey(KeyCode.LeftShift));
            }
            /*if (Mathf.Abs(mouseDown.x - Input.mousePosition.x) <= 1 && Mathf.Abs(mouseDown.z - Input.mousePosition.z) <= 1)
            {
                
            }
            else/* if(selected.Count == 0)*/
            /*{
                waypoint.SetActive(false);
            }*/
                
        }
            
        /*if (GameObject.Find("EventSystem").GetComponent<FlowField>().hasGenerated() && AttackedObject == null)
        {
            float3[] flowfield = GameObject.Find("EventSystem").GetComponent<FlowField>().getFlowField();
            setTarget(flowfield);

        }*/
    }

    bool click()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayhit;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }
            
        if(Physics.Raycast(ray, out rayhit))
        {
            if (rayhit.transform.gameObject.layer == 3 || rayhit.transform.gameObject.layer == 8)
            {
                UnitLoad unit = rayhit.transform.GetChild(0).GetComponent<UnitLoad>();
                if (unit.OutputUnit().getGroup() != group || a_move)
                    Attack(rayhit.transform.gameObject);
                else
                    AddtoTroop(rayhit.transform.gameObject);
                return false;
            }
            target = rayhit.point;
            if (selected.Count == 0)
                return false;
            AttackedObject = null;
            transform.position = new Vector3(rayhit.point.x, 0f, rayhit.point.z);
            waypoint.SetActive(true);
            return true;
        }
        return false;
    }

    public void Attack(GameObject t)
    {
        if (!active)
            return;
        waypoint.SetActive(false);
        AttackedObject = t;
        foreach (GameObject gameobj in selected)
        {
            gameobj.GetComponent<InstructionQueue>().clearAndExecute(new Instruction(1, t));
        }
    }

    public void AddtoTroop(GameObject t)
    {
        if (!active)
            return;
        waypoint.SetActive(false);
        MovementControl control;
        Attack attack;
        UnitLoad unit = t.transform.GetChild(0).GetComponent<UnitLoad>();
        InstructionQueue queue;
        foreach(GameObject gameobj in selected)
        {
            if (gameobj.TryGetComponent<MovementControl>(out control))
                control.cancelTarget();
            if (gameobj.transform.GetChild(3).TryGetComponent<Attack>(out attack))
                attack.stopAttack();
            if (gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10 == unit.OutputUnit().getTroopType())
            {
                
                queue = gameobj.GetComponent<InstructionQueue>();
                queue.clearAndExecute(new Instruction(2, t));
            }
        }
    }

    public void Active(bool a)
    {
        active = a;
    }

    public void setAMove(bool a)
    {
        a_move = a;
    }

    public bool isAMove()
    {
        return a_move;
    }

    List<Vector3> targetGetsurrounding(Vector3 target)
    {
        List<Vector3> result = new List<Vector3>();
        int x = (int)target.x;
        int z = (int)target.z;
        Vector3 coordinate;
        for (int i = x - 15; i <= x + 15; i += 15)
            for (int j = z - 15; j <= z + 15; j += 15)
            {
                if (i == x && z == j)
                    continue;
                coordinate = new Vector3(i, 0, j);
                coordinate.y = Terrain.activeTerrain.SampleHeight(coordinate);
                if (!HasObstacle.hasObstacle(new float3(i, 0, j), 15, true) && !OutOfBounds(new Vector3(i, 0, j)))
                {
                    result.Add(new Vector3(i, 0, j));
                }
                    
            }
                
        return result;
    }

    bool OutOfBounds(Vector3 pos)
    {
        if (pos.x < -500 || pos.x >= 500 || pos.z < -500 || pos.z >= 500)
            return true;
        return false;
    }

    void setTarget(float3[] flowfield, bool planned)
    {
        GameObject gameobject = GameObject.Find("EventSystem");
        float3 t = gameobject.GetComponent<FlowField>().getTarget();
        int gridLength = gameobject.GetComponent<FlowField>().getGridLength();

        List<GameObject> list = new List<GameObject>(selected);
        if (list.Count == 1 && list[0].transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isBuilding())
        {
            if (list[0].GetComponent<ConstructionFunction>() != null)
                list[0].GetComponent<ConstructionFunction>().SetTarget(target);
            else
                transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        /*foreach (GameObject gameobj in list)
        {
            if (!gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
                AllFlyUnit = false;
        }
        /*if (AllFlyUnit)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].GetComponent<InstructionQueue>().clearQueue();
                list[i].GetComponent<InstructionQueue>().addInstruction(new Instruction(0, new Vector3(t.x, t.y, t.z)));
            }
            return;
        }*/
        GameObject current;
        List<Vector3> neighbors;
        List<Vector3> openSet = new List<Vector3>();
        List<Vector3> closeSet = new List<Vector3>();
        Queue<Vector3> queue = new Queue<Vector3>();
        queue.Enqueue(target);
        openSet.Add(target);
        Vector3 currentVector = target;

        while(list.Count > 0 && queue.Count > 0)
        {
            if(queue.Count > 0)
                currentVector = queue.Dequeue();
            if (closeSet.Contains(currentVector))
                continue;
            
            current = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //current.GetComponent<InstructionQueue>().clearQueue();
            //current.GetComponent<VehicleControl>().MoveTo(currentVector, false);
            //current.GetComponent<VehicleControl>().MoveTo(flowfield, new float3((int)currentVector.x, (int)currentVector.y, (int)currentVector.z), gridLength);
            if (a_move)
                current.GetComponent<InstructionQueue>().addInstructionToFront(new Instruction(5, currentVector, flowfield));
            else if(planned)
                current.GetComponent<InstructionQueue>().addInstruction(new Instruction(3, currentVector, flowfield));
            else
                current.GetComponent<InstructionQueue>().clearAndExecute(new Instruction(3, currentVector, flowfield));

            current.transform.GetChild(0).GetComponent<UnitLoad>().setAbilityActive(false);

            //current.GetComponent<VehicleControl>().MoveTo(currentVector, false);
            neighbors = targetGetsurrounding(currentVector);
            closeSet.Add(currentVector);
            //GameObject gameobj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //gameobj.transform.position = currentVector + new Vector3(0, 5, 0);
            foreach (Vector3 neighbor in neighbors)
                queue.Enqueue(neighbor);
        }
        if (a_move)
            a_move = false;
    }

    public Vector3 getPosition()
    {
        return target;
    }
}
