using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataHandlers;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Casting;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Entities.Characters
{
    public abstract class BaseCharacter : BaseGenericEntity<CharacterData>, IPIRDetectable
    {
        private CastAction activeCastAction;
        private List<GameObject> items;
        private List<GameObject> itemIcons;
        private Animator animator;
        private int activeItemIndex;

        protected float RotationSpeed = 90;
        protected int Money;
        protected bool IsMoving;

        private AudioSource footstepAudio;
        protected float Speed = 1f;
        protected CharacterController Controller;

        public bool IsActive { get; set; }

        public Camera Camera { get; set; }

        public List<GameObject> Items
        {
            get
            {
                if (items == null)
                {
                    Object[] allItems = ResourcesHolder.Instance.AllItems;
                    Object[] allItemIcons = ResourcesHolder.Instance.AllItemsIcons;
                    // Instantiate 1 GameObject per 1 itemName in the Data
                    items = Data.ItemNames.Select(itemName =>
                        Instantiate(allItems.First(obj => obj.name == itemName) as GameObject)).ToList();
                    itemIcons = Data.ItemNames.Select(itemName =>
                        Instantiate(allItemIcons.First(obj => obj.name == itemName) as GameObject)).ToList();
                }
                return items;
            }
            set
            {
                items = value;
                Data.ItemNames = items.Select(item => item.name).ToArray();
            }
        }

        /// <summary>
        /// AI Model position. (This is NOT world position.)
        /// </summary>
        public IntegerTuple Position
        {
            get { return Map.GetClosestTile(transform.position).Position; }
        }

        public override void StartGame()
        {
            base.StartGame();
            footstepAudio = gameObject.AttachAudioSource("Footstep");
            Controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            animator.speed = 2;

            foreach (GameObject item in Items)
            {
                InitializeItem(item);
            }
            foreach (GameObject itemIcon in itemIcons)
            {
                itemIcon.SetActive(false);
            }
            GameObject activeItem = GetActiveItem();
            if (activeItem != null)
                activeItem.SetActive(true);
        }

        protected override void UpdateGame()
        {
            base.UpdateGame();
            if (Input.GetMouseButton(0))
            {
                OnLeftButtonClick();
            }
            if (transform.position.y != 0)
            {
                // Just to ensure avoiding flying.
                Vector3 groundPosition = transform.position;
                groundPosition.y = 0;
                transform.position = groundPosition;
            }
            animator.SetBool("Moving", IsMoving);
            if (IsMoving)
            {
                if (!footstepAudio.isPlaying)
                {
                    footstepAudio.Play();
                    animator.speed = 2;
                }
                if (activeCastAction != null)
                {
                    activeCastAction.Interrupt();
                }
            }
            else
            {
                footstepAudio.Stop();
                animator.speed = 1;
            }
        }

        private void OnLeftButtonClick()
        {
            if (IsActive)
            {
                Vector3 localEulerAngles = Camera.transform.localEulerAngles;
                float deltaX = Time.deltaTime * RotationSpeed * (-Input.GetAxis("Mouse Y"));
                float deltaY = Time.deltaTime * RotationSpeed * (Input.GetAxis("Mouse X"));
                float potentialRotationX = localEulerAngles.x + deltaX;
                float potentialRotationY = localEulerAngles.y + deltaY;

                float xRotation = potentialRotationX > 75 && potentialRotationX < 360 - 75
                    ? localEulerAngles.x
                    : potentialRotationX;
                float yRotation = potentialRotationY > 75 && potentialRotationY < 360 - 75
                    ? localEulerAngles.y
                    : potentialRotationY;
                
                Camera.transform.localEulerAngles = new Vector3(xRotation, yRotation, 0);

            }
        }

        public void AddItem(GameObject itemObject)
        {
            Items.Add(itemObject);
            Object[] allItemIcons = ResourcesHolder.Instance.AllItemsIcons;
            GameObject itemIconObject = Instantiate(
                allItemIcons.First(obj => obj.name == itemObject.GetComponent<ItemEntity>().PrefabName) as GameObject);
            itemIconObject.transform.position = new Vector3(0, -10, 0);
            itemIcons.Add(itemIconObject);
            GameObject activeObject = GetActiveItem();
            if (activeObject != null) activeObject.SetActive(false);
            activeItemIndex = Items.Count - 1;
            InitializeItem(itemObject);
            GetActiveItem().SetActive(true);
        }

        protected void MoveForward()
        {
            IsMoving = true;
            var transformedDir = transform.TransformDirection(Speed * Time.deltaTime * Vector3.forward);
            Controller.Move(transformedDir);
        }

        private void InitializeItem(GameObject item)
        {
            Transform finger =
                transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L/Finger_01");
            item.transform.position = finger.position;
            item.transform.parent = finger;
            var baseItem = item.GetComponent<BaseItem>();
            baseItem.DefaultLocalPosition = item.transform.localPosition;
            item.SetActive(false);
        }

        /// <summary>
        /// Not used right now, but ready for password objects.
        /// </summary>
        /// <param name="passwordOpenableObject"></param>
        public abstract void RequestPassword(IPasswordOpenable passwordOpenableObject);
        /// <summary>
        /// Not used right now, but ready for password objects.
        /// </summary>
        public abstract void InterruptRequestPassword();
        public abstract void RequestCard(CardReaderEntity cardReader);

        public GameObject GetActiveItem()
        {
            if (Items.Count == 0)
            {
                return null;
            }
            else
            {
                if (IsActive)
                {
                    CurrentGame.CurrentItemIcon.sprite = itemIcons[activeItemIndex].GetComponent<SpriteRenderer>().sprite;
                }
                CurrentGame.CurrentItemIcon.gameObject.SetActive(IsActive);
                
                return Items[activeItemIndex];
            }
        }

        public void Interact(RaycastHit[] raycastHits)
        {
            bool isInterctableNow;
            GameObject closestGameObject = GetClosestInteractableHitObject(raycastHits, out isInterctableNow);
            if (isInterctableNow)
            {
                var interactable = closestGameObject.GetComponent<IInteractable>();
                interactable.Interact(this);
            }
        }

        public void InteractWith(GameObject interacted, Action successAction = null)
        {
            if (!InteractHelper.AreCloseToInteract(gameObject, interacted)) return;
            interacted.GetComponent<IInteractable>().Interact(this, successAction);
        }

        protected GameObject GetClosestInteractableHitObject(RaycastHit[] raycastHits,
            out bool isInteractableNow)
        {
            bool unusedOutParam;
            return GetClosestInteractableHitObject(raycastHits, out unusedOutParam, out isInteractableNow);
        }

        /// <param name="raycastHits"></param>
        /// <param name="isInteractable"> 
        /// True iff the closest hit gameobject has a script that implements <see cref="IInteractable"/>
        /// </param>
        /// <param name="isInteractableNow"> 
        /// True iff <paramref name="isInteractable"/> AND the distance between the character and
        /// the closest hit gameobject is lesser than <see cref="Constants.INTERACTABLE_DISTANCE"/>.
        /// </param>
        /// <returns> The closest gameobjects among all that were hit. </returns>
        protected GameObject GetClosestInteractableHitObject(RaycastHit[] raycastHits,
            out bool isInteractable,
            out bool isInteractableNow)
        {
            RaycastHit closestHit = default(RaycastHit);
            foreach (RaycastHit hit in raycastHits)
                if (hit.transform.gameObject.HasScriptOfType(typeof(IInteractable)) &&
                    (closestHit.Equals(default(RaycastHit)) ||
                     Vector3.Distance(transform.position, hit.transform.position) <
                     Vector3.Distance(transform.position, closestHit.transform.position))
                )
                {
                    closestHit = hit;
                }

            if (closestHit.Equals(default(RaycastHit)))
            {
                isInteractable = isInteractableNow = false;
                return null;
            }
            GameObject closestGameObject = closestHit.transform.gameObject;
            isInteractable = true;
            Vector3 closestPosition = closestGameObject.transform.position;
            Vector3 myPosition = transform.position;
            var distanceToClosest = Vector3.Distance(new Vector3(closestPosition.x, closestPosition.z), new Vector3(myPosition.x, myPosition.z));
            isInteractableNow = distanceToClosest < Constants.Constants.INTERACTABLE_DISTANCE &&
                                isInteractable;
            return closestGameObject;
        }

        public bool CanInteractWith(GameObject gameObject)
        {
            if (!gameObject.HasScriptOfType(typeof(IInteractable))) return false;
            var distance = Vector3.Distance(gameObject.transform.position, transform.position);
            return distance < Constants.Constants.INTERACTABLE_DISTANCE;
        }

        protected void ChangeActiveItem()
        {
            if (Items.Count == 0) return;
            GetActiveItem().SetActive(false);
            activeItemIndex = (activeItemIndex + 1) % Items.Count;
            GetActiveItem().SetActive(true);
        }

        public void AllowUnlocking(Action unlockOnce)
        {
            // Animation of unlocking.
            unlockOnce();
        }

        public virtual void ObtainMoney()
        {
            Money++;
        }
        
        public void Attack()
        {
            animator.SetTrigger("Attack" + 3 + "Trigger");
            //StartCoroutine(_LockMovementAndAttack(0, .6f));
        }

        public void AttackForSeconds(float seconds)
        {
            StartCoroutine(AttackForSecondsEnumerator(seconds));
        }

        private IEnumerator AttackForSecondsEnumerator(float seconds)
        {
            float lastTime = seconds;
            while (seconds > 0)
            {
                if ((int) lastTime != (int) seconds)
                {
                    Attack();
                }
                lastTime = seconds;
                seconds -= Time.deltaTime;
                yield return null;
            }
        }

        public bool NavigateTo(TileNode tileNode)
        {
            Vector3 target = Map.Tiles.Get(tileNode.Position).transform.position;
            bool isCloseEnough = Vector3.Distance(transform.position, target) < 0.05f;

            transform.LookAt(target);
            MoveForward();
            
            return isCloseEnough;
        }

        public void ActivateItem<T>() where T : BaseItem
        {
            activeItemIndex = Items.IndexOf(Items.First(item => item.HasScriptOfType<T>()));
        }

        public void Cast(CastAction action)
        {
            activeCastAction = action;
            IsMoving = false;
        }

        public void FinishCasting()
        {
            activeCastAction = null;
        }

        public abstract void Log(string line);

        //Animation Events - just to enable Mecanim animations work => Do NOT delete!
        protected void Hit()
        {

        }

        protected void FootL()
        {

        }

        protected void FootR()
        {

        }

        protected void Jump()
        {

        }

        protected void Land()
        {

        }

        public void GoalsCompleted()
        {
            CurrentGame.GoalsCompleted(this);
        }
    }
}
