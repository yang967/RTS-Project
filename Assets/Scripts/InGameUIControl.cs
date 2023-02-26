using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InGameUIControl : MonoBehaviour
{
    GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getUnit(string name)
    {
        manager.setCurrentUnit(GameObject.Find("Main Camera").GetComponent<UnitSelection>().getCurrentSelected());
        string NAME = transform.parent.name;
        int index = NAME[NAME.Length - 1] - '0';
        manager.SetUpUnitBar(name, index - 1);
    }

    public void setEquipment(string name)
    {
        manager.setCurrentUnitEquipment(manager.getNewEquipment(name));
    }

    public void resetEquipment()
    {
        manager.resetCurrentUnitEquipment();
    }

    public void SelectedBar()
    {
        manager.SetUpSelectedBar();
    }

    public void removeAllTroop()
    {
        manager.RemoveTroop(-1);
    }

    public void setupTroopRemoval()
    {
        manager.SetUpTroopRemovalBar();
        //transform.parent.Find("SelectionBar").gameObject.SetActive(true);
    }

    public void removeTroopByIndex()
    {
        int index = transform.parent.name[transform.parent.name.Length - 1] - '0';
        manager.RemoveTroop(index);
    }

    public void removeTroop()
    {
        int index = gameObject.name[gameObject.name.Length - 1] - '0';
        manager.RemoveTroop(index - 1);
    }

    public void CallEquipmentSelection(string name)
    {
        int index = name[name.Length - 1] - '0';
        name = name.Substring(0, name.Length - 1);
        GameObject Functionbar = GameObject.Find("UI").transform.GetChild(2).GetChild(0).gameObject;
        if (!Functionbar.name.Equals("DetailBar(Clone)"))
            return;
        GameObject bar = Functionbar.transform.Find("SelectionBar").gameObject;
        if (bar.activeSelf)
            bar.SetActive(false);
        else
        {
            List<Equipment> list = manager.getEquipment(name)[index];
            GameObject gameobj, button;
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].getName().Equals(""))
                {
                    gameobj = Resources.Load(name + "DefaultButton" + index) as GameObject;
                }
                else
                {
                    gameobj = Resources.Load(list[i].getName() + "Button") as GameObject;
                }
                
                button = Instantiate(gameobj, bar.transform.GetChild(i));
                button.GetComponent<RectTransform>().sizeDelta = button.transform.parent.GetComponent<RectTransform>().sizeDelta;
            }
            bar.SetActive(true);
        } 
    }

    public void setupConstructionBar()
    {
        manager.setConstructionBar();
    }

    public void construct(string name)
    {
        manager.construct(name);
    }

    public void produce(string name)
    {
        manager.Produce(name);
    }
}
