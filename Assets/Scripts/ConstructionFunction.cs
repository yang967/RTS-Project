using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionFunction : MonoBehaviour
{
    Building building;
    HashSet<string> production;
    float time;
    float CurrentTime;
    bool producing;
    string current;
    [SerializeField] Vector3 OutPos;
    Vector3 target;
    LinkedList<string> queue;
    Dictionary<string, int> dict;
    int group;

    // Start is called before the first frame update
    void Start()
    {
        building = GameObject.Find("EventSystem").GetComponent<GameManager>().getConstruction(transform.GetChild(0).name);
        production = new HashSet<string>();
        dict = new Dictionary<string, int>();
        queue = new LinkedList<string>();
        string[] p = building.getProduction();
        for(int i = 0; i < p.Length; i++)
        {
            production.Add(p[i]);
            dict.Add(p[i], i);
        }
        producing = false;
        time = 0;
        CurrentTime = 0;
        current = "";
        target = OutPos;
        group = transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup();
    }

    // Update is called once per frame
    void Update()
    {
        if (!producing)
            return;
        if(Time.time - time >= CurrentTime)
        {
            producing = false;
            GameObject unit = Instantiate(Resources.Load(current) as GameObject, OutPos + transform.position, Quaternion.identity);
            queue.RemoveFirst();
            unit.transform.GetChild(0).GetComponent<UnitLoad>().SetGroup(group);
            if (target.x != OutPos.x || target.z != OutPos.z)
                unit.GetComponent<InstructionQueue>().addInstruction(new Instruction(0, target));
            if(queue.Count > 0)
            {
                Unit u = GameObject.Find("EventSystem").GetComponent<GameManager>().getNewUnit(queue.First.Value);
                time = Time.time;
                CurrentTime = u.getProduceTime();
                producing = true;
                current = queue.First.Value;
            }
        }
    }

    public void produce(string name)
    {
        if (!production.Contains(name))
            return;
        queue.AddLast(name);
        if (producing)
            return;
        Unit unit = GameObject.Find("EventSystem").GetComponent<GameManager>().getNewUnit(name);
        time = Time.time;
        CurrentTime = unit.getProduceTime();
        producing = true;
        current = name;
    }

    public void SetTarget(Vector3 pos)
    {
        if (pos.x > 500 || pos.x < -500 || pos.z > 500 || pos.z < -500)
            return;
        target = pos;
    }

    public bool Contain(string name)
    {
        return production.Contains(name);
    }

    public float getProgress()
    {
        if (!producing)
            return 0;
        return (Time.time - time) / CurrentTime;
    }

    public LinkedList<string> getProductionQueue()
    {
        return queue;
    }

    public int getIndex(string name)
    {
        if (!production.Contains(name))
            return -1;
        return dict[name];
    }
    
    public int getProductionUnitCount()
    {
        return building.getProduction().Length;
    }

    public Building getBuilding()
    {
        return building;
    }
}
