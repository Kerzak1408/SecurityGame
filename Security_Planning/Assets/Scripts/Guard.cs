using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour {

    GameObject Canvas;
    float speed;

    // Use this for initialization
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        speed = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (Input.GetKey(KeyCode.LeftArrow))
           controller.Move(speed * Vector3.left);
        if (Input.GetKey(KeyCode.RightArrow))
            controller.Move(speed * Vector3.right);
        if (Input.GetKey(KeyCode.DownArrow))
            controller.Move(speed * Vector3.down);
        if (Input.GetKey(KeyCode.UpArrow))
            controller.Move(speed * Vector3.up);
    }


}
