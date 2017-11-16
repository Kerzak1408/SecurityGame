using System;
using System.Collections.Generic;
using System.Linq;
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
            RaycastHit interactableHit =
                raycastHits.FirstOrDefault(hit => hit.transform.gameObject.HasScriptOfType(typeof(IInteractable)));
            if (!interactableHit.Equals(default(RaycastHit)))
            {
                IInteractable interactable = interactableHit.transform.gameObject.GetComponent<IInteractable>();
                interactable.Interact(this);
            }
        }

        protected void ChangeWeapon()
        {
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
