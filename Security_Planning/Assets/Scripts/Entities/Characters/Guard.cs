using Assets.Scripts.Entities.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Entities.Characters
{
    public class Guard : BaseCharacter
    {
        GameObject Canvas;
        float speed;
        float rotationSpeed;

        GameObject inputPassword;

        IPasswordOpenable passwordOpenableObject;

        CharacterController controller;
        private ResourcesHolder resourcesHolder;
        private Animator animator;
        private Rigidbody rigidBody;

        // Use this for initialization
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            resourcesHolder = ResourcesHolder.Instance;
            rotationSpeed = 90;
            Canvas = GameObject.Find("Canvas");
            inputPassword = GameObject.Find("InputField_Password");
            inputPassword.SetActive(false);
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        private void Update()
        {
            animator.SetBool("Moving", false);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                ChangeWeapon();
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
                animator.SetBool("Moving", true);
                var transformedDir = transform.TransformDirection(speed * Vector3.left);
                controller.Move(transformedDir);
            }
            if (Input.GetKey(KeyCode.E))
            {
                animator.SetBool("Moving", true);
                var transformedDir = transform.TransformDirection(speed * Vector3.right);
                controller.Move(transformedDir);
            }
        
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("Moving", true);
                var transformedDir = transform.TransformDirection(speed * Vector3.forward);
                controller.Move(transformedDir);
            }
            if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("Moving", true);
                controller.transform.Rotate(Time.deltaTime * new Vector3(0, rotationSpeed, 0));
            }
            if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("Moving", true);
                controller.transform.Rotate(Time.deltaTime * new Vector3(0, -rotationSpeed, 0));
            } 
            if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("Moving", true);
                var transformedDir = transform.TransformDirection(speed * Vector3.back);
                controller.Move(transformedDir);
            }
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
            //transform.Rotate(-90, 0, 0);

            var guardPosition = transform.position;
            transform.position = new Vector3(guardPosition.x, guardPosition.y, 0);
            Camera mainCamera = Camera.main;
            mainCamera.transform.position = guardPosition;
            mainCamera.orthographicSize = 1;
            mainCamera.transform.parent = transform;
            mainCamera.transform.localPosition = new Vector3(0, 1.3f, 0.22f);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

        //Animation Events
        void Hit()
        {

        }

        void FootL()
        {

        }

        void FootR()
        {

        }

        void Jump()
        {

        }

        void Land()
        {

        }
    }


}
