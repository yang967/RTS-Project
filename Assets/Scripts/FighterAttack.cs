using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAttack : Attack
{
    int stage;
    Transform model;
    InstructionQueue insQueue;
    float Dis;
    Quaternion rotation;

    protected override void Start()
    {
        base.Start();
        model = transform.parent.GetChild(0);
        stage = 0;
        control = transform.parent.GetComponent<MovementControl>();
        insQueue = transform.parent.GetComponent<InstructionQueue>();
    }

    private void Update()
    {

        if (target != null && target.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getHitPoint() <= 0)
        {
            TargetList.Remove(target);
            TargetUnits.Remove(target.name);
            target = null;
        }
    }

    protected override void FixedUpdate()
    {
        if (!active)
            return;

        Vector3 next_pos;

        switch(stage)
        {
            case 0:
                if (target == null)
                {
                    if (TargetList.Count == 0)
                        return;
                    HashSet<GameObject>.Enumerator em = TargetList.GetEnumerator();
                    em.MoveNext();
                    GameObject first = em.Current;
                    if(first == null)
                    {
                        TargetList.Remove(first);
                        return;
                    }
                    if (control.isIdle())
                    {
                        insQueue.addInstruction(new Instruction(1, first));
                    }

                }
                try
                {
                    float x_difference = target.transform.position.x - transform.parent.position.x;
                    float z_difference = target.transform.position.z - transform.parent.position.z;
                    Dis = x_difference * x_difference + z_difference * z_difference;
                    if (Dis > 30 * 30)
                    {
                        stage++;
                        insQueue.addInstructionToFront(new Instruction(0, target.transform.position));
                    }
                    else
                    {
                        insQueue.addInstructionToFront(new Instruction(0, transform.parent.position + transform.parent.forward * 40));
                        return;
                    }
                } catch(MissingReferenceException)
                {
                    return;
                } catch(System.NullReferenceException)
                {
                    return;
                }
                break;
            case 1:
                try
                {
                    float x_difference = target.transform.position.x - transform.parent.position.x;
                    float z_difference = target.transform.position.z - transform.parent.position.z;
                    Dis = x_difference * x_difference + z_difference * z_difference;
                    if(Dis < 20 * 20)
                    {
                        insQueue.addInstructionToFront(new Instruction(0, transform.parent.position + transform.parent.forward * 40));
                        stage = 0;
                        return;
                    }
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    Quaternion target_rotation = Quaternion.LookRotation(direction);
                    if (Mathf.Abs(transform.eulerAngles.y - target_rotation.eulerAngles.y) <= 2 && TargetList.Contains(target) && in_range)
                        stage++;
                    else if (control.isIdle())
                        insQueue.addInstructionToFront(new Instruction(0, target.transform.position));
                } catch(MissingReferenceException)
                {
                    stage++;
                } catch(System.NullReferenceException)
                {
                    stage = 5;
                }
                break;
            case 2:
                spin();
                stage++;
                break;
            case 3:
                model.rotation = Quaternion.Slerp(model.rotation, rotation, 20 * Time.deltaTime);
                next_pos = model.transform.position + model.transform.forward * thisUnit.getSpeed() * Time.deltaTime;
                model.position = new Vector3(model.position.x, next_pos.y, model.position.z);
                if (Mathf.Abs(model.eulerAngles.x - rotation.eulerAngles.x) <= 0.5f)
                {
                    model.rotation = rotation;
                    stage++;
                }
                break;
            case 4:
                try
                {
                    fight();
                    model.LookAt(target.transform);
                    transform.parent.eulerAngles = new Vector3(0, model.eulerAngles.y, 0);
                    next_pos = model.transform.position + model.transform.forward * thisUnit.getSpeed() * Time.deltaTime;
                    model.position = new Vector3(model.position.x, next_pos.y, model.position.z);
                    Vector3 pos = target.transform.position;
                    if ((Mathf.Abs(transform.position.x - pos.x) <= 5f && Mathf.Abs(transform.position.z - pos.z) <= 5f) || model.position.y <= 7)
                    {
                        Vector3 parent_pos = transform.parent.position;
                        insQueue.addInstructionToFront(new Instruction(0, parent_pos + transform.parent.forward * 40));
                        stage++;
                    }
                    if (control.isIdle())
                        insQueue.addInstructionToFront(new Instruction(0, transform.parent.position + transform.parent.forward * 5));
                } catch(MissingReferenceException)
                {
                    Vector3 parent_pos = transform.parent.position;
                    control.MoveTo(parent_pos + transform.parent.forward * 40, false);
                    stage = 5;
                } catch(System.NullReferenceException)
                {
                    Vector3 parent_pos = transform.parent.position;
                    control.MoveTo(parent_pos + transform.parent.forward * 40, false);
                    stage = 5;
                }
                break;
            case 5:
                back_spin();
                stage++;
                break;
            case 6:
                model.rotation = Quaternion.Slerp(model.rotation, rotation, 10 * Time.deltaTime);
                next_pos = model.transform.position + model.transform.forward * thisUnit.getSpeed() * Time.deltaTime;
                model.position = new Vector3(model.position.x, next_pos.y, model.position.z);
                if (control.isIdle())
                    insQueue.addInstructionToFront(new Instruction(0, transform.parent.position + transform.parent.forward * 5));
                if (model.position.y >= thisUnit.getHeight())
                {
                    model.position = new Vector3(model.position.x, thisUnit.getHeight(), model.position.z);
                    stage++;
                }
                break;
            case 7:
                normal();
                stage++;
                break;
            case 8:
                model.rotation = Quaternion.Slerp(model.rotation, rotation, 20 * Time.deltaTime);
                if (control.isIdle())
                    insQueue.addInstructionToFront(new Instruction(0, transform.parent.position + transform.parent.forward * 5));
                if (model.eulerAngles.x >= -10f && model.eulerAngles.x < 180)
                {
                    model.eulerAngles = transform.parent.eulerAngles;
                    stage++;
                }
                break;
            default:
                stage = 0;
                break;
        }
    }

    void spin()
    {
        try
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rotation = Quaternion.LookRotation(direction);
        } catch(MissingReferenceException) 
        {
            stage = 5;
            return; 
        }
    }

    void back_spin()
    {
        rotation = model.rotation;
        rotation.eulerAngles = new Vector3(-30, rotation.eulerAngles.y, rotation.eulerAngles.z);
    }

    void normal()
    {
        rotation = model.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (target != null && other.name.Equals(target.name))
        {
            Debug.Log(target.name);
            inRange = true;
            return;
        }*/
        if (other.gameObject.layer != 3 && other.gameObject.layer != 8)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() == thisUnit.getGroup())
            return;
        if (thisUnit.getAntiType()[other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getUnitType() / 10] == -1)
            return;
        if (TargetList.Contains(other.gameObject))
            return;
        TargetList.Add(other.gameObject);
    }

    /*private void OnTriggerExit(Collider other)
    {
        //if (target == null)
          //return;
        if (other.gameObject.layer != 3 && other.gameObject.layer != 8)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() != thisUnit.getGroup())
        {
            if (target == null || !target.Equals(other.gameObject))
            {
                TargetList.Remove(other.gameObject);
                if (target != null && target.Equals(other.gameObject))
                {
                    target = null;
                    locked = false;
                }
            }
        }
    }*/

    void fight()
    {
        try
        {
            if (Time.time - time >= thisUnit.getFireRate() && thisUnit.getDamage() > 0)
            {
                for (int i = 0; i < thisUnit.getBurst(); i++)
                {
                    if (animator != null)
                        animator.SetTrigger("Attacking");
                    if (index >= bulletOut.Length)
                        index = 0;
                    GameObject bGameobj = Instantiate(bullet, bulletOut[index].transform.position, Quaternion.identity);
                    bGameobj.GetComponent<Bullet>().Attack(target, bulletOut[index], thisUnit.getDamage(), thisUnit.getArmorPenetration(), thisUnit.getAccuracy(), thisUnit.getAntiType());
                    index++;
                }
                time = Time.time;
            }
        } catch(MissingReferenceException)
        {
            stage = 5;
            return;
        } catch(System.NullReferenceException)
        {
            stage = 5;
            return;
        }
    }

    public override void setTarget(GameObject gameobj)
    {
        //Debug.Log("set");
        base.setTarget(gameobj);
        stage = 5;
    }

    public override void stopAttack()
    {
        base.stopAttack();
        stage = 5;
    }
}
