using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundUnitCollision : MonoBehaviour
{

    Ray ray1, ray2, ray3, rayWalkable;
    bool active;
    bool detect;
    bool close;
    public LayerMask ignore;
    int selectedCount;
    //GameObject gameObject;
    List<GameObject> triggerList;
    float distance;
    MovementControl control;
    Vector3 target;
    bool set;
    Quaternion rotation;
    Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        detect = true;
        triggerList = new List<GameObject>();
        control = transform.GetComponent<MovementControl>();
        close = true;
        selectedCount = 0;
        set = false;
        rotation = transform.rotation;
        active = true;
        unit = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
        //gameObject = transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;
        Turn();
        if (!set)
            return;
        /*distance = Mathf.Sqrt((target.x - transform.position.x) * (target.x - transform.position.x) + (target.z - transform.position.z) * (target.z - transform.position.z));
        if (selectedCount != 1 && distance < selectedCount * 10 && !control.isIdle() && close)
        {
            control.MoveTo(target, false);
            close = false;
            detect = false;
        }*/
        if (distance < 1)
            set = false;
        //ifWalkable(target);
        //detectForward();
        
    }

    void Turn()
    {
        if (!control.isIdle()) {
            rotation = transform.rotation;
            detect = true;
            return;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
    }

    public void setTarget(Vector3 target)
    {
        this.target = new Vector3(target.x, target.y, target.z);
        set = true;
    }

    public void setCount(int count)
    {
        selectedCount = count;
    }

    public void setActive(bool active)
    {
        this.active = active;
    }


    private void OnTriggerStay(Collider other)
    {
        if (!active)
            return;
        MovementControl vc = transform.GetComponent<MovementControl>();
        //if (transform.GetChild(0).name.Equals("APC") && other.gameObject.layer == 3)
          //  Debug.Log(other.name);
        if (!vc.isIdle() || !detect || (other.gameObject.layer != 3 && other.gameObject.layer != 8))
            return;
        UnitLoad load = transform.GetChild(0).GetComponent<UnitLoad>();
        GameObject[] troops = load.getTroops();
        int currentTroops = load.getCurrentTroops();
        for (int i = 0; i < currentTroops; i++)
            if (troops[i].Equals(other.gameObject))
                return;
        //Debug.Log(transform == null);
        //if ((other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10) < unit.getUnitType() / 10)
          //  return;
        Vector3 move = Vector3.MoveTowards(transform.position, other.transform.position, -transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getSpeed() * Time.deltaTime);
        
        if (vc.isIdle())
            vc.cancelTarget();
        move.y = transform.position.y;
        transform.position = move;
        Vector3 direction = (other.transform.position - transform.position).normalized;
        rotation = Quaternion.LookRotation(direction);
        rotation.eulerAngles += new Vector3(0, 180, 0);
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);

        /*float3[] flowfield = vc.getFlowField();
        if (flowfield != null && !control.isIdle())
        {
            int next = normalized(transform.position);
            if (Mathf.Abs(transform.position.x - flowfield[next].x) > 1 || Mathf.Abs(transform.position.z - flowfield[next].z) > 1)
                control.setCurrentTarget(new Vector3(flowfield[next].x, flowfield[next].y, flowfield[next].z));
        }*/
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    int normalized(Vector3 position)
    {
        int x, z;
        if (Mathf.Abs((int)position.x % 10) >= 10 / 2)
            x = ((int)position.x / 10 + ((int)position.x > 0 ? 1 : -1)) * 10;
        else
            x = (int)position.x / 10 * 10;
        if (Mathf.Abs(position.z % 10) >= 10 / 2)
            z = ((int)position.z / 10 + ((int)position.z > 0 ? 1 : -1)) * 10;
        else
            z = (int)position.z / 10 * 10;
        return (x / 10 + 50) * 100 + z / 10 + 50;
    }

    public void resetTriggerList()
    {
        triggerList = new List<GameObject>();
    }

    public void setDetect(bool d)
    {
        detect = d;
    }

    public void setClose(bool c)
    {
        close = c;
    }
}
