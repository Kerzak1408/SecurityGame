using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Guard : BaseCharacter {

    GameObject Canvas;
    float speed;

    GameObject inputPassword;

    IPasswordOpenable passwordOpenableObject;

    // Use this for initialization
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        inputPassword = GameObject.Find("InputField_Password");
        inputPassword.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (passwordOpenableObject != null)
            {
                string password = inputPassword.GetComponent<InputField>().text;
                passwordOpenableObject.EnterPassword(password, this);
            }
        }
        if (Input.GetKey(KeyCode.Z))
        {
            speed = 0.025f;
        }
        else
        {
            speed = 0.01f;
        }
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

    public override void RequestPassword(IPasswordOpenable passwordOpenableObject)
    {
        var inputField = inputPassword.GetComponent<InputField>();
        inputField.text = "";
        inputPassword.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();

        this.passwordOpenableObject = passwordOpenableObject;
    }

    public override void InterruptRequestPassword()
    {
        if (inputPassword != null)
        {
            inputPassword.SetActive(false);
            passwordOpenableObject = null;
        }
    }
}
