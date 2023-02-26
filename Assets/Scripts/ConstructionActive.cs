using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConstructionActive : MonoBehaviour
{
    bool active;
    Building current;
    Camera cam;
    int gridlength;
    int mapwidth;
    int[] costfield;
    GameManager manager;
    GameObject currentObj;
    FlowField flowfield;
    //int ignored;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        flowfield = GameObject.Find("EventSystem").GetComponent<FlowField>();
        gridlength = flowfield.getGridLength();
        mapwidth = 1000 / gridlength;
        costfield = flowfield.getCostField();
        manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        //ignored = 1 << 6;
        //ignored = ~ignored;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;
        if(Input.GetMouseButtonUp(1))
        {
            unset();
            return;
        }
        if(currentObj.GetComponentInChildren<MeshRenderer>().material.color == Color.green && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
        {
            construct();
            return;
        }
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayhit;
        if(Physics.Raycast(ray, out rayhit))
        {
            bool clear = true;
            float x;
            if (current.getLength() % 2 != 0)
                x = Mathf.RoundToInt(rayhit.point.x / gridlength);
            else
                x = Mathf.RoundToInt(rayhit.point.x / gridlength) + 0.5f;
            float y;
            if (current.getWidth() % 2 != 0)
                y = Mathf.RoundToInt(rayhit.point.z / gridlength);
            else
                y = Mathf.RoundToInt(rayhit.point.z / gridlength) + 0.5f;
            float length = (current.getLength() - 1) / 2;
            float width = (current.getWidth() - 1) / 2;
            int c;
            for(int i = (int)(x - length); i <= (int)(x + length); i++)
            {
                for (int j = (int)(y - width); j <= (int)(y + width); j++)
                {
                    if (OutOfBound(i, j))
                    {
                        Debug.Log(i + " " + j + " Out of Bound");
                        clear = false;
                        break;
                    }
                    c = (i + 500 / gridlength) * mapwidth + j + 500 / gridlength;
                    //Debug.Log(i + " " + j + " " + c + " " + costfield[c]);
                    if (costfield[c] == 65535)
                    {
                        clear = false;
                        break;
                    }
                }
                if (!clear)
                    break;
            }
            Vector3 pos = new Vector3(x * gridlength, 0, y * gridlength);
            pos.y = Terrain.activeTerrain.SampleHeight(pos) + manager.getUnitHeight(current.getName());
            currentObj.transform.position = pos;
            if (!clear)
                for (int i = 0; i < currentObj.transform.childCount; i++)
                    currentObj.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.red;
            else
                for (int i = 0; i < currentObj.transform.childCount; i++)
                    currentObj.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.green;
        }
    }

    bool OutOfBound(int x, int y)
    {
        if (x >= 50 || x <= -50 || y >= 50 || y <= -50)
            return true;
        return false;
    }

    public void set(Building construction)
    {
        GameObject.Find("Main Camera").GetComponent<UnitSelection>().setActive(false);
        GameObject.Find("Target").GetComponent<Target>().Active(false);
        if (currentObj != null)
            Destroy(currentObj);
        current = construction;
        active = true;
        GameObject gameobj = Resources.Load(construction.getName() + "Model") as GameObject;
        currentObj = Instantiate(gameobj);
    }

    public void unset()
    {
        GameObject.Find("Main Camera").GetComponent<UnitSelection>().setActive(true);
        GameObject.Find("Target").GetComponent<Target>().Active(true);
        current = null;
        active = false;
        if (currentObj != null)
            Destroy(currentObj);
    }

    void updateCostField()
    {
        float x = currentObj.transform.position.x / gridlength;
        float y = currentObj.transform.position.z / gridlength;
        float length = (current.getLength() - 1) / 2;
        float width = (current.getWidth() - 1) / 2;
        int c;
        for (int i = (int)(x - length); i <= (int)(x + length); i++)
        {
            for (int j = (int)(y - width); j <= (int)(y + width); j++)
            {
                c = (i + 500 / gridlength) * mapwidth + j + 500 / gridlength;
                flowfield.updateCostField(c, 65535);
            }
        }
    }

    public void construct()
    {
        if (current == null)
            return;
        GameObject obj = Resources.Load(current.getName()) as GameObject;
        GameObject construction = Instantiate(obj, currentObj.transform.position, Quaternion.identity);
        construction.GetComponent<Construction>().startConstruction();
        List<GameObject> temp = new List<GameObject>();
        temp.Add(construction);
        manager.setCurrentUnit(temp);
        manager.SetUpConstructingBar(current.getName(), 0);
        updateCostField();
        unset();
    }
}
