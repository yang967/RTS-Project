using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelection : MonoBehaviour
{
    private List<GameObject> rayHit;
    private Vector2 MousePosition;
    private Vector2 dMousePosition;
    private Vector2 uMousePosition;

    private Camera cam;
    int group;

    GameManager manager;

    [SerializeField]private LayerMask UnitLayer;
    [SerializeField]private RectTransform selection;
    [SerializeField] GameObject FunctionBar;

    bool active;

    Target target;

    void Start()
    {
        cam = transform.gameObject.GetComponent<Camera>();
        group = cam.gameObject.GetComponent<CameraControl>().getGroup();
        rayHit = new List<GameObject>();
        selection.sizeDelta = new Vector2(0, 0);
        selection.anchoredPosition = new Vector2(-960, -(1920 * ((float)Screen.height / Screen.width)) / 2);
        selection.gameObject.SetActive(false);
        active = true;
        manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        target = GameObject.Find("Target").GetComponent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        if (!active)
        {
            dMousePosition = Input.mousePosition;
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            MousePosition = Input.mousePosition / new Vector2(Screen.width, Screen.height) * new Vector2(1920, 1920 * ((float)Screen.height / Screen.width));
            dMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Mathf.Abs(MousePosition.x - Input.mousePosition.x / Screen.width * 1920) > Screen.width / 300 && Mathf.Abs(MousePosition.y - Input.mousePosition.y / Screen.height * (1920 * (float)Screen.height / Screen.width)) > Screen.height / 150)
            {
                selection.gameObject.SetActive(true);
                UpdateBox(Input.mousePosition / new Vector2(Screen.width, Screen.height) * new Vector2(1920, 1920 * ((float)Screen.height / Screen.width)));
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            selection.gameObject.SetActive(false);
            if (Mathf.Abs(currentMousePosition.x - dMousePosition.x) > 2 && Mathf.Abs(currentMousePosition.y - dMousePosition.y) > 2)
            {
                uMousePosition = Input.mousePosition;
                BoxSelection();
            }
            else
                SingleSelection();
                
        }
    }

    void SingleSelection()
    {
        RaycastHit ray = new RaycastHit();
        if (EventSystem.current.IsPointerOverGameObject() || target.isAMove())
            return;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, Mathf.Infinity, UnitLayer))
        {
            if ((ray.collider.gameObject.layer != 3 && ray.collider.gameObject.layer != 8) || ray.collider.GetType() == typeof(SphereCollider))
                return;
            if (ray.collider.transform.parent != null)
                return;
            if (rayHit.Count > 0 && !Input.GetKey(KeyCode.LeftControl))
                CleanRayHit();
            
            if (!contains(ray))
                rayHit.Add(ray.transform.gameObject);
            if (ray.collider.gameObject.layer == 8)
            {
                manager.setCurrentUnit(rayHit);
                manager.SetUpUnitBar(ray.collider.transform.GetChild(0).name, 0);
            }
            else
                manager.SetUpSelectedBar();
            ray.collider.GetComponent<ClickMe>().Clicked();
            ray.collider.GetComponent<InstructionQueue>().SetRouteActive(true);
        }
    }
    void UpdateBox(Vector2 mPosition)
    {
        float width = mPosition.x - MousePosition.x;
        float height = mPosition.y - MousePosition.y;

        selection.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selection.anchoredPosition = MousePosition + new Vector2(width / 2 - 960, height / 2 - 1920 * ((float)Screen.height / Screen.width) / 2);
    }

    void BoxSelection()
    {
        float width = uMousePosition.x - dMousePosition.x;
        float height = uMousePosition.y - dMousePosition.y;
        Vector2 min = dMousePosition + new Vector2(width / 2, height / 2) - new Vector2(Mathf.Abs(width), Mathf.Abs(height)) / 2;
        Vector2 max = dMousePosition + new Vector2(width / 2, height / 2) + new Vector2(Mathf.Abs(width), Mathf.Abs(height)) / 2;
        bool selected = false;
        List<Unit> units = manager.getUnitList();
        List<GameObject> gameobjs = new List<GameObject>();
        GameObject current;
        Vector3 pos, screenpos;
        float[] position;
        foreach (Unit unit in units)
        {
            if (unit.getUnitType() / 10 == 3)
                continue;
            position = unit.getPosition();
            pos = new Vector3(position[0], position[1], position[2]);
            screenpos = cam.WorldToScreenPoint(pos);
            current = GameObject.Find("Unit" + unit.getID());
            if (!current.GetComponentInChildren<MeshRenderer>().enabled)
                continue;
            if (screenpos.x > min.x && screenpos.y > min.y && screenpos.x < max.x && screenpos.y < max.y)
            {
                selected = true;
                current.GetComponent<ClickMe>().Clicked();
                current.GetComponent<InstructionQueue>().SetRouteActive(true);
                gameobjs.Add(GameObject.Find("Unit" + unit.getID()));
            }
            
        }
        if (selected)
        {
            CleanRayHit(gameobjs);
            rayHit.AddRange(gameobjs);
            manager.SetUpSelectedBar();
        }
        selection.sizeDelta = new Vector2(0, 0);
        selection.anchoredPosition = new Vector2(-960, -(1920 * ((float)Screen.height / Screen.width)) / 2);
        target.setAMove(false);
    }

    public void setActive(bool active)
    {
        this.active = active;
    }
    public List<GameObject> getCurrentSelected()
    {
        return rayHit;
    }

    public void removeAt(GameObject gameobj)
    {
        rayHit.Remove(gameobj);
        manager.setCurrentUnit(rayHit);
    }

    void CleanRayHit(List<GameObject> list)
    {
        foreach (GameObject gameobj in rayHit)
        {
            try
            {
                if (!list.Contains(gameobj))
                {
                    gameobj.GetComponent<InstructionQueue>().SetRouteActive(false);
                    gameobj.GetComponent<ClickMe>().UnClicked();
                }
            }
            catch(MissingReferenceException)
            {
                continue;
            }
        }
        rayHit = new List<GameObject>();
    }

    void CleanRayHit()
    {
        foreach (GameObject gameobj in rayHit)
        {
            try
            {
                gameobj.GetComponent<InstructionQueue>().SetRouteActive(false);
                gameobj.GetComponent<ClickMe>().UnClicked();
            }
            catch(MissingReferenceException)
            {
                continue;
            }
        }
        rayHit = new List<GameObject>();
    }

    bool contains(RaycastHit ray)
    {
        foreach (GameObject gameobj in rayHit)
        {
            if (ray.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getID() == gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getID())
                return true;
        }
        return false;
    }

    public List<GameObject> getSelectedWithName(string name)
    {
        List<GameObject> result = new List<GameObject>();
        foreach(GameObject gameobj in rayHit)
        {
            if (gameobj.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getType().Equals(name))
                result.Add(gameobj);
        }
        return result;
    }
}
