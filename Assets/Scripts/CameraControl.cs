using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    float speed = 70f;
    float zoomspeed = 10.0f;
    float rotateSpeed;
    Player player;
    float maxHeight = 60f;
    float minHeight = 4f;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0f, maxHeight, 0f);
        transform.eulerAngles = new Vector3(65f, 0, 0f);
        player = GameObject.Find("EventSystem").GetComponent<GameManager>().PopCurrentPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.y >= Screen.height - 5)
            Moveforward();
        if (Input.mousePosition.y <= 5)
            Movebackward();
        if (Input.mousePosition.x >= Screen.width - 5)
            Moveright();
        if (Input.mousePosition.x <= 5)
            Moveleft();
        zoom();
    }

    public int getGroup()
    {
        return player.getGroup();
    }

    void Moveforward() {
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= speed * Time.deltaTime;
        transform.position += forwardMove;
    }
    void Movebackward() {
        Vector3 backwardMove = transform.forward;
        backwardMove.y = 0;
        backwardMove.Normalize();
        backwardMove *= -speed * Time.deltaTime;
        transform.position += backwardMove;
    }
    void Moveleft() {
        Vector3 leftMove = transform.right * -speed * Time.deltaTime;
        transform.position += leftMove;
    }
    void Moveright() {
        Vector3 rightMove = transform.right * speed * Time.deltaTime;
        transform.position += rightMove;
    }
    void zoom() {
        float zoomvalue = -zoomspeed * Input.GetAxis("Mouse ScrollWheel");
        if (transform.position.y >= maxHeight && zoomvalue > 0)
            zoomvalue = 0;
        else if (transform.position.y <= minHeight && zoomvalue < 0)
            zoomvalue = 0;
        if (transform.position.y >= maxHeight)
            transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
        if (transform.position.y <= minHeight)
            transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);
        Vector3 verticalMove = new Vector3(0f, zoomvalue, 0f);
        transform.position += verticalMove;
        
    }
}
