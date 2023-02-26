using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectScript : MonoBehaviour
{
    public void displaySelected(List<GameObject> selected)
    {
        for(int i = 0; i < selected.Count; i++)
        {
            /*GameObject button = Resources.Load(selected[i].transform.GetChild(0).name + "Button") as GameObject;
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            Instantiate(button, transform.GetChild(i));*/
            GameObject gameobj = Instantiate(Resources.Load("UnitButton"), transform.GetChild(i)) as GameObject;
            string name = selected[i].transform.GetChild(0).name;
            gameobj.GetComponent<UnitButton>().SetName(name);
            gameobj.GetComponent<RawImage>().texture = Resources.Load(name + "Image") as Texture;
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            gameobj.GetComponent<RectTransform>().sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y);
            
        }
    }
}
