using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTest : MonoBehaviour
{
    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        //rect.anchoredPosition = new Vector2(-950, -590);
    }

    // Update is called once per frame
    void Update()
    {
        //rect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float width = 1920 * ((float)Screen.height / Screen.width);
        rect.anchoredPosition = Input.mousePosition / new Vector2(Screen.width, Screen.height) * new Vector2(1920, width) - new Vector2(960, width / 2);
    }
}
