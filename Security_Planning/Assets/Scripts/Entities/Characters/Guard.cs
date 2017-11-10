using Assets.Scripts.Entities.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Entities.Characters
{
    public class Guard : BaseCharacter {

        GameObject Canvas;
        float speed;
        float rotationSpeed;

        GameObject inputPassword;

        IPasswordOpenable passwordOpenableObject;

        CharacterController controller;

        // Use this for initialization
        private void Start()
        {
            rotationSpeed = 90;
            Canvas = GameObject.Find("Canvas");
            inputPassword = GameObject.Find("InputField_Password");
            inputPassword.SetActive(false);
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnFirstLeftButtonClick();
            } 
            else if (Input.GetMouseButton(0))
            {
                OnLeftButtonClick();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (passwordOpenableObject != null)
                {
                    string password = inputPassword.GetComponent<InputField>().text;
                    passwordOpenableObject.EnterPassword(password, this);
                }
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = 0.04f;
            }
            else
            {
                speed = 0.02f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                var transformedDir = transform.TransformDirection(speed * Vector3.left);
                controller.Move(transformedDir);
            }
            if (Input.GetKey(KeyCode.E))
            {
                var transformedDir = transform.TransformDirection(speed * Vector3.right);
                controller.Move(transformedDir);
            }
        
            if (Input.GetKey(KeyCode.W))
            {
                var transformedDir = transform.TransformDirection(speed * Vector3.forward);
                controller.Move(transformedDir);
            }
            if (Input.GetKey(KeyCode.D))
            {
                controller.transform.Rotate(Time.deltaTime * new Vector3(0, rotationSpeed, 0));
            }
            if (Input.GetKey(KeyCode.A))
            {
                controller.transform.Rotate(Time.deltaTime * new Vector3(0, -rotationSpeed, 0));
            } 
            if (Input.GetKey(KeyCode.S))
            {
                var transformedDir = transform.TransformDirection(speed * Vector3.back);
                controller.Move(transformedDir);
            }
        }

        private void OnFirstLeftButtonClick()
        {

        }

        private void OnLeftButtonClick()
        {
            var delta = new Vector3(Time.deltaTime * rotationSpeed * (-Input.GetAxis("Mouse Y")), 0, 0);
            Camera.main.transform.Rotate(delta);
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

        public override void RequestCard(CardReaderEntity cardReader)
        {
            cardReader.VerifyCard();
        }

        public override void StartGame()
        {
            base.StartGame();
            transform.Rotate(-90, 0, 0);
            var guardPosition = transform.position;
            Camera mainCamera = Camera.main;
            mainCamera.transform.position = guardPosition;
            mainCamera.orthographicSize = 1;
            mainCamera.transform.parent = transform;
            mainCamera.transform.localRotation = Quaternion.identity;
            
        }
    }
}
