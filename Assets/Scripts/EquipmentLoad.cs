using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentLoad : MonoBehaviour
{
    Equipment equipment;
    [SerializeField] GameObject Barrel;
    [SerializeField] GameObject BarrelBody;
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject[] bulletOut;
    string Type;
    bool finished;

    // Start is called before the first frame update
    void Awake()
    {
        Type = gameObject.name;
        Type = Type.Replace("(Clone)", "");
        equipment = new Equipment(GameObject.Find("EventSystem").GetComponent<GameManager>().getNewEquipment(Type));
        finished = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Equipment getEquipment()
    {
        return equipment;
    }

    public GameObject getBarrel()
    {
        return Barrel;
    }

    public GameObject getBarrelBody()
    {
        return BarrelBody;
    }

    public GameObject[] getBulletOut()
    {
        return bulletOut;
    }

    public GameObject getBullet()
    {
        return Bullet;
    }

    public bool isFinished()
    {
        return finished;
    }
}
