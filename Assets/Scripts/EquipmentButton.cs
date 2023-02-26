using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentButton : MonoBehaviour
{
    string name_;

    public void setName(string name)
    {
        name_ = name;
        GetComponent<RawImage>().texture = Resources.Load(name_ + "Image") as Texture2D;
    }

    public void setEquipment()
    {
        GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        manager.setCurrentUnitEquipment(manager.getNewEquipment(name_));
    }
}
