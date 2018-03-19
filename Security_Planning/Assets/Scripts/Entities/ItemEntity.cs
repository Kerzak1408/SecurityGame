using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Entities
{
    public class ItemEntity : BaseEntityWithBaseData, IInteractable
    {
        public void Interact(BaseCharacter character, Action sucsess = null)
        {
            character.AddItem(gameObject);
            Destroy(this);
        }
    }
}
