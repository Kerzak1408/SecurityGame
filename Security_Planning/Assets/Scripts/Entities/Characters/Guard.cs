using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Entities.Characters
{
    public class Guard : BaseCharacter
    {
        GameObject Canvas;
        
        float rotationSpeed;

        GameObject inputPassword;

        IPasswordOpenable passwordOpenableObject;

        
        private ResourcesHolder resourcesHolder;
        private Rigidbody rigidBody;
        private Camera guardCamera;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            rigidBody = GetComponent<Rigidbody>();
            resourcesHolder = ResourcesHolder.Instance;
            rotationSpeed = 90;
            Canvas = GameObject.Find("Canvas");
            inputPassword = GameObject.Find("InputField_Password");
            inputPassword.SetActive(false);
            
            gameObject.AddComponent<ConstantForce>().force = Vector3.forward;
            guardCamera = GetComponentInChildren<Camera>();


        }

        // Update is called once per frame
        protected override void Update()
        {
            
            isMoving = false;
            
            Ray ray = guardCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = Physics.RaycastAll(ray);

            UpdateCursor(raycastHits);

            if (Input.GetMouseButtonUp(1))
            {
                Interact(raycastHits);
            } 
            else if (Input.GetMouseButton(0))
            {
                OnLeftButtonClick();
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                ChangeActiveItem();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (passwordOpenableObject != null)
                {
                    string password = inputPassword.GetComponent<InputField>().text;
                    passwordOpenableObject.EnterPassword(password, this);
                }
            }

            if (Input.GetKey(KeyCode.Q))
            {
                isMoving = true;

                var transformedDir = transform.TransformDirection(speed * Time.deltaTime * Vector3.left);
                controller.Move(transformedDir);
            }
            if (Input.GetKey(KeyCode.E))
            {
                isMoving = true;
                var transformedDir = transform.TransformDirection(speed * Time.deltaTime * Vector3.right);
                controller.Move(transformedDir);
            }
        
            if (Input.GetKey(KeyCode.W))
            {
                MoveForward();
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
                isMoving = true;
                var transformedDir = transform.TransformDirection(speed * Time.deltaTime * Vector3.back);
                controller.Move(transformedDir);
            }
            base.Update();

        }

        private void UpdateCursor(RaycastHit[] raycastHits)
        {
            bool isInteractable;
            bool isInteractableNow;
            GetClosestInteractableHitObject(raycastHits, out isInteractable, out isInteractableNow);
               
            Texture2D cursorTexture;
            if (isInteractableNow)
            {
                cursorTexture = resourcesHolder.CogwheelTexture;
            }
            else if (isInteractable)
            {
                cursorTexture = resourcesHolder.CogwheelPaleTaxture;
            }
            else
            {
                cursorTexture = null;
            }
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        private void OnLeftButtonClick()
        {
            var delta = new Vector3(Time.deltaTime * rotationSpeed * (-Input.GetAxis("Mouse Y")), 0, 0);
            guardCamera.transform.Rotate(delta);
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
            //transform.Rotate(-90, 0, 0);

            var guardPosition = transform.position;
            transform.position = new Vector3(guardPosition.x, guardPosition.y, 0);
            Camera guardCamera = GetComponentInChildren<Camera>();
            guardCamera.transform.position = guardPosition;
            guardCamera.orthographicSize = 1;
            guardCamera.transform.parent = transform;
            guardCamera.transform.localPosition = new Vector3(0, 1.3f, 0.22f);
            guardCamera.transform.localRotation = Quaternion.identity;
            IsActive = true;
        }

        public override void ObtainMoney()
        {
            base.ObtainMoney();
            CurrentGame.TextMoney.text = money.ToString();
        }


    }


}
