using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCallButton : MonoBehaviour
{
    string name_;
    int index_;


    public void set(string name, int index)
    {
        name_ = name;
        index_ = index;
        GetComponent<RawImage>().texture = Resources.Load(name_ + "Image") as Texture;
    }

    public void CallEquipment()
    {
        GameObject FunctionBar = transform.parent.parent.gameObject;
        GameObject bar = FunctionBar.transform.Find("SelectionBar").gameObject;
        GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        if (bar.activeSelf)
            bar.SetActive(false);
        else
        {
            List<Equipment> list = manager.getEquipment(name_)[index_];
            for (int i = 0; i < list.Count; i++)
            {
                GameObject button;
                //if (list[i].getName().Equals(""))
                //    button = Instantiate(Resources.Load(name + "DefaultButton" + index_) as GameObject, bar.transform.GetChild(i));
                //else
                button = Instantiate(Resources.Load("EquipmentButton") as GameObject, bar.transform.GetChild(i));
                button.GetComponent<EquipmentButton>().setName(list[i].getName());
                button.GetComponent<RectTransform>().sizeDelta = button.transform.parent.GetComponent<RectTransform>().sizeDelta;
            }
            bar.SetActive(true);
        }
    }
}
