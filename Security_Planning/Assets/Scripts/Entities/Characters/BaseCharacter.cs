using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Entities.Characters
{
    public abstract class BaseCharacter : BaseGenericEntity<CharacterData>, IPIRDetectable
    {
        private List<GameObject> items;
        private List<GameObject> itemIcons;
        private Animator animator;
        private int activeItemIndex;
        protected int money;
        protected bool isMoving;
        private AudioSource footstepAudio;
        protected float speed = 0.04f;
        protected CharacterController controller;

        protected List<GameObject> Items
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

        protected override void Start()
        {
            base.Start();
            footstepAudio = gameObject.AttachAudioSource("Footstep");
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            animator.speed = 2;
        }

        protected override void Update()
        {
            base.Update();
            if (transform.position.y != 0)
            {
                // Just to ensure avoiding flying.
                Vector3 groundPosition = transform.position;
                groundPosition.y = 0;
                transform.position = groundPosition;
            }
            animator.SetBool("Moving", isMoving);
            if (isMoving)
            {
                if (!footstepAudio.isPlaying)
                {
                    footstepAudio.Play();
                    animator.speed = 2;
                }
            }
            else
            {
                footstepAudio.Stop();
                animator.speed = 1;
            }
        }

        protected void MoveForward()
        {
            isMoving = true;
            var transformedDir = transform.TransformDirection(speed * Vector3.forward);
            controller.Move(transformedDir);
        }

        public override void StartGame()
        {
            animator = GetComponent<Animator>();
            Transform finger =
                transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L/Finger_01");
            foreach (GameObject item in Items)
            {
                //item.transform.position = transform.position + new Vector3(0.2f,-0.375f,0.2f);
                item.transform.position = finger.position;
                item.transform.parent = finger;
                var baseItem = item.GetComponent<BaseItem>();
                baseItem.DefaultLocalPosition = item.transform.localPosition;
                item.SetActive(false);
            }
            foreach (GameObject itemIcon in itemIcons)
            {
                itemIcon.SetActive(false);
            }
            GameObject activeItem = GetActiveItem();
            if (activeItem != null)
                activeItem.SetActive(true);
        }

        public abstract void RequestPassword(IPasswordOpenable passwordOpenableObject);
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
                CurrentGame.CurrentItemIcon.sprite = itemIcons[activeItemIndex].GetComponent<SpriteRenderer>().sprite;
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

        public void InteractWith(GameObject gameObject)
        {
            if (!CanInteractWith(gameObject)) return;
            gameObject.GetComponent<IInteractable>().Interact(this);
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
            var distanceToClosest = Vector3.Distance(closestGameObject.transform.position, transform.position);
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

        protected void ChangeWeapon()
        {
            if (Items.Count == 0) return;
            GetActiveItem().SetActive(false);
            activeItemIndex = (activeItemIndex + 1) % Items.Count;
            GetActiveItem().SetActive(true);
        }

        public void AllowUnlocking(Action UnlockOnce)
        {
            // Animation of unlocking.
            UnlockOnce();
        }

        public virtual void ObtainMoney()
        {
            money++;
        }
        
        public void Attack()
        {
            animator.SetTrigger("Attack" + 3 + "Trigger");
            StartCoroutine(_LockMovementAndAttack(0, .6f));
        }

        public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime)
        {
            yield return new WaitForSeconds(delayTime);
            animator.SetBool("Moving", false);
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            //inputVec = new Vector3(0, 0, 0);
            animator.applyRootMotion = true;
            yield return new WaitForSeconds(lockTime);
            //canAction = true;
            //canMove = true;
            animator.applyRootMotion = false;
        }

        public bool NavigateTo(TileNode tileNode)
        {
            Vector3 target = CurrentGame.Map.Tiles.Get(tileNode.Position).transform.position;
            transform.LookAt(target);
            MoveForward();
            return (Vector3.Distance(transform.position, target) < 0.4f);
        }

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
    }
}
