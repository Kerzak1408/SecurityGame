using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters
{
    public abstract class BaseCharacter : BaseGenericEntity<CharacterData>
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
            foreach (GameObject item in Items)
            {
                item.transform.position = transform.position + new Vector3(0.2f,-0.075f,0.2f);
                item.transform.parent = transform;
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
    }
}
