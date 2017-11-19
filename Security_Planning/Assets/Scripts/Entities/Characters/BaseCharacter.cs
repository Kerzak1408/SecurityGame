using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants;
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
        private List<GameObject> items;
        private int activeItemIndex;

        protected List<GameObject> Items
        {
            get
            {
                if (items == null)
                {
                    Object[] allItems = ResourcesHolder.Instance.AllItems;
                    // Instantiate 1 GameObject per 1 itemName in the Data
                    items = Data.ItemNames.Select(itemName =>
                        Instantiate(allItems.First(obj => obj.name == itemName) as GameObject)).ToList();
                }
                return items;
            }
            set
            {
                items = value;
                Data.ItemNames = items.Select(item => item.name).ToArray();
            }
        }

        public override void StartGame()
        {
            Transform finger = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L/Finger_01");
            foreach (GameObject item in Items)
            {
                //item.transform.position = transform.position + new Vector3(0.2f,-0.375f,0.2f);
                item.transform.position = finger.position;
                item.transform.parent = finger;
                BaseItem baseItem = item.GetComponent<BaseItem>();
                baseItem.DefaultLocalPosition = item.transform.localPosition;
                item.SetActive(false);
            }
            GameObject activeItem = GetActiveItem();
            if (activeItem != null)
            {
                activeItem.SetActive(true);
            }
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
                return Items[activeItemIndex];
            }
        }

        public void Interact(RaycastHit[] raycastHits)
        {
            bool isInterctableNow;
            GameObject closestGameObject = GetClosestInteractableHitObject(raycastHits, out isInterctableNow);
            if (isInterctableNow)
            {
                IInteractable interactable = closestGameObject.GetComponent<IInteractable>();
                interactable.Interact(this);
            }
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
            {
                if (hit.transform.gameObject.HasScriptOfType(typeof(IInteractable)) &&
                    (closestHit.Equals(default(RaycastHit)) ||
                        Vector3.Distance(transform.position, hit.transform.position) <
                        Vector3.Distance(transform.position, closestHit.transform.position))
                    )
                {
                    closestHit = hit;
                }
            }
            if (closestHit.Equals(default(RaycastHit)))
            {
                isInteractable = isInteractableNow = false;
                return null;
            }
            GameObject closestGameObject = closestHit.transform.gameObject;
            isInteractable = true;
            float distanceToClosest = Vector3.Distance(closestGameObject.transform.position, transform.position);
            isInteractableNow = distanceToClosest < Constants.Constants.INTERACTABLE_DISTANCE &&
                                isInteractable;
            return closestGameObject;
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


    }
}
