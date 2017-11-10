using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters
{
    public abstract class BaseCharacter : BaseGenericEntity<CharacterData>
    {
        private List<GameObject> items;

        public List<GameObject> Items
        {
            get
            {
                if (items == null)
                {
                    Object[] allItems = ResourcesHolder.Instance.AllItems;
                    // Instantiate 1 GameObject per 1 itemName in the Data
                    items = Data.ItemNames.Select(itemName =>
                        Instantiate(allItems.First(obj => obj.name == itemName) as GameObject)).ToList();
                    items = new List<GameObject>();
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
        }

        public abstract void RequestPassword(IPasswordOpenable passwordOpenableObject);
        public abstract void InterruptRequestPassword();
        public abstract void RequestCard(CardReaderEntity cardReader);
    }
}
