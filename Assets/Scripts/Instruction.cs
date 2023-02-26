using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Instruction
{
    int type;
    bool sub;
    Vector3 position;
    GameObject target;
    float3[] flowfield;
    bool BoolValue;
    
    public Instruction(int type, Vector3 position)
    {
        this.type = type;
        this.position = new Vector3(position.x, position.y, position.z);
        sub = false;
        BoolValue = false;
    }

    public Instruction(int type, bool b, Vector3 position)
    {
        this.type = type;
        this.position = new Vector3(position.x, position.y, position.z);
        sub = false;
        BoolValue = b;
    }

    public Instruction(int type, GameObject target)
    {
        this.type = type;
        this.target = target;
        sub = false;
        BoolValue = false;
    }

    public Instruction(int type, Vector3 position, float3[] flowfield)
    {
        this.type = type;
        this.position = new Vector3(position.x, position.y, position.z);
        if (flowfield == null)
            this.flowfield = null;
        else
        {
            this.flowfield = new float3[flowfield.Length];
            flowfield.CopyTo(this.flowfield, 0);
        }
        sub = false;
        BoolValue = false;
    }

    public void SetSubTask()
    {
        sub = true;
    }

    public Instruction(int type)
    {
        this.type = type;
    }

    public int getInstructionType()
    {
        return type;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public GameObject getTarget()
    {
        return target;
    }

    public float3[] getFlowField()
    {
        return flowfield;
    }

    public bool isSubTask()
    {
        return sub;
    }
}
