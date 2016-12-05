using UnityEngine;
using System.Collections;

public class GateOpen : MonoBehaviour {

    Vector3 defaultPosition;
    Vector3 localScale;
    float speed;
    bool isReturning;
    bool firstTime;

    void Start()
    {
        defaultPosition = transform.position;
        localScale = transform.lossyScale;
        firstTime = false;
        speed = 2f;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");

        isReturning = false;
    }

    void OnTriggerStay(Collider other)
    {

        if (transform.position.y < defaultPosition.y + localScale.y)
            transform.position += speed * Vector3.up * Time.deltaTime;
    }

    void OnTriggerExit(Collider other)
    {
        isReturning = true;
    }

    void Update()
    {

        if (isReturning && transform.position.y > defaultPosition.y)
            transform.position += speed * Vector3.down * Time.deltaTime;
        Debug.Log(isReturning + " currentY = " + transform.position.y +" defaultY = " + defaultPosition.y);
    }
}
