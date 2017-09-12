using UnityEngine;
using System.Collections;

public class GateOpen : MonoBehaviour
{

    public bool horizontal;
    public GameObject gate;

    Vector3 defaultPosition;
    Vector3 lossyScale;
    Vector3 translationDirection;
    float speed;
    bool isReturning;
    Axis axis;

    private void Start()
    {
        defaultPosition = gate.transform.position;
        lossyScale = gate.transform.lossyScale;
        speed = 4f;
        translationDirection = horizontal ? Vector3.right : Vector3.up;
        axis = horizontal ? Axis.X : Axis.Y;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");

        isReturning = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
        bool gateFullyOpen = true;

        if (gate.transform.position.GetVectorCoord(axis) < defaultPosition.GetVectorCoord(axis) + lossyScale.GetVectorCoord(axis))
        {
            gateFullyOpen = false;
        }

        if (!gateFullyOpen)
        {
            gate.transform.position += speed * translationDirection * Time.deltaTime;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        isReturning = true;
    }

    private void Update()
    {
        if (isReturning)
        {
            bool gateFullyClosed = true;

            if (gate.transform.position.GetVectorCoord(axis) > defaultPosition.GetVectorCoord(axis))
            {
                gateFullyClosed = false;
            }

            if (!gateFullyClosed)
            {
                gate.transform.position -= speed * translationDirection * Time.deltaTime;
                if (gate.transform.position.GetVectorCoord(axis) < defaultPosition.GetVectorCoord(axis))
                {
                    gate.transform.position = defaultPosition;
                }
            }
        }   
    }

}
