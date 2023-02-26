using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using System;

public class AbilityButton : MonoBehaviour
{
    AbilityLoad load;
    int indx;

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
    }

    public void SetLoad(AbilityLoad load, int indx)
    {
        this.load = load;
        this.indx = indx;
        UpdateStats();
    }

    void UpdateStats()
    {
        AbilityData data = load.get(indx);
        GetComponent<RawImage>().texture = Resources.Load(data.getName() + "Image") as Texture;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = load.getCharge(indx) + "";
        if (load.getCD(indx) == -1)
            transform.GetChild(2).gameObject.SetActive(false);
        else
        {
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (int)(load.getCD(indx) * 100) + "%";
        }
    }

    public bool Usable()
    {
        return load.Usable(indx);
    }

    public bool Use()
    {
        return load.Use(indx);
    }
}
