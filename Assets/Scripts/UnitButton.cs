using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
    string Name;

    public void SetName(string name)
    {
        Name = name;
    }

    public void getUnit()
    {
        GameManager manager = GameObject.Find("EventSystem").GetComponent<GameManager>();
        manager.setCurrentUnit(GameObject.Find("Main Camera").GetComponent<UnitSelection>().getCurrentSelected());
        string NAME = transform.parent.name;
        int index = NAME[NAME.Length - 1] - '0';
        manager.SetUpUnitBar(Name, index - 1);
    }
}
