using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityActive : MonoBehaviour
{
    [SerializeField] int index;
    Camera cam;
    Vector3 target;
    bool set;
    Target Target;
    UnitSelection selection;
    Vector3 mouseDown;
    GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject camera = GameObject.Find("Main Camera");
        cam = camera.GetComponent<Camera>();
        selection = camera.GetComponent<UnitSelection>();
        set = false;
        Target = GameObject.Find("Target").GetComponent<Target>();
        manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    // Update is called once per frame
    void Update()
    {
        if (!set)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = Input.mousePosition;
            Target.Active(false);
            selection.setActive(false);
        }
        if (Input.GetMouseButtonUp(1) && set)
        {
            set = false;
            Target.Active(true);
            selection.setActive(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Mathf.Abs(mouseDown.x - Input.mousePosition.x) <= 1 && Mathf.Abs(mouseDown.z - Input.mousePosition.z) <= 1)
                click();
            /*else
            {
                Target.Active(true);
                selection.setActive(true);
            }*/
        }
            
    }

    public void start()
    {
        if (!GetComponent<AbilityButton>().Usable())
            return;
        set = true;
        Target.Active(false);
        selection.setActive(false);
    }

    void click()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayhit;
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if(Physics.Raycast(ray, out rayhit))
        {
            if (rayhit.collider.gameObject.layer == 3 || rayhit.collider.gameObject.layer == 8)
            {
                target = rayhit.collider.gameObject.transform.position;
                target.y = Terrain.activeTerrain.SampleHeight(target);
            }
            else if (rayhit.collider.GetType() == typeof(TerrainCollider))
                target = rayhit.point;
            else
                return;
            set = false;
            Target.Active(true);
            selection.setActive(true);
            GameObject current = GameObject.Find("EventSystem").GetComponent<GameManager>().getCurrent();
            UnitLoad load;
            if(current.transform.GetChild(0).TryGetComponent<UnitLoad>(out load))
            {
                load.ActiveAbility(index, target);
            }
            else
            {
                Debug.Log("Target does not have an Ability stick to it");
            }
            
        }
    }
}
