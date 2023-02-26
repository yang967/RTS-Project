using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPosition : MonoBehaviour
{
    Transform cam;
    RectTransform RectTransform;
    Vector3 map;
    float ratio;
    bool inMap;
    // Start is called before the first frame update
    void Start()
    {
        ratio = 300 / 1920.0f * Screen.width / 2;
        cam = GameObject.Find("Main Camera").transform;
        RectTransform = gameObject.GetComponent<RectTransform>();
        map = transform.parent.position;
        inMap = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mPos = Input.mousePosition;
        if(Input.GetMouseButtonDown(0) &&
            mPos.x < map.x + ratio && mPos.x > map.x - ratio &&
            mPos.y < map.y + ratio && mPos.y > map.y - ratio)
            inMap = true;
        if (inMap && Input.GetMouseButton(0) &&
            mPos.x < map.x + ratio && mPos.x > map.x - ratio &&
            mPos.y < map.y + ratio && mPos.y > map.y - ratio)
        {
            cam.position = new Vector3((mPos.x - map.x) / ratio * 500, cam.position.y, (mPos.y - map.y) / ratio * 500 - 10);
        }
        if (Input.GetMouseButtonUp(0))
            inMap = false;
        float x = cam.position.x / 500 * ratio, y = cam.position.z / 500 * ratio + 10;
        x = Mathf.Max(-ratio, x);
        x = Mathf.Min(ratio, x);
        y = Mathf.Max(-ratio, y);
        y = Mathf.Min(ratio, y);
        transform.position = map + new Vector3(x, y, 0);
    }
}
