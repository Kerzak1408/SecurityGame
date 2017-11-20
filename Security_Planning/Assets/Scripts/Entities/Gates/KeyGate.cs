using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class KeyGate : BaseSlidingGate, IInteractable
    {
        protected override void Start()
        {
            base.Start();
            Lock();
        }

        public void Interact(BaseCharacter character)
        {
            GameObject activeItemObject = character.GetActiveItem();
            if (activeItemObject.HasScriptOfType<KeyItem>())
            {
                UnlockOnce();
            }
        }
    }
}
