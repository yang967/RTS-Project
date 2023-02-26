using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMe : MonoBehaviour
{
    [SerializeField]
    private GameObject selected;
    // Start is called before the first frame update
    void Start()
    {
        selected.SetActive(false);
    }

    // Update is called once per frame
    public void Clicked() {
        selected.SetActive(true);
    }

    public void UnClicked() {
        selected.SetActive(false);
    }
}
