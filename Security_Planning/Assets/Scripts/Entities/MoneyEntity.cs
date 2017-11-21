using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class MoneyEntity : BaseEntityWithBaseData, IInteractable
    {
        public void Interact(BaseCharacter character)
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Money"), transform.position);
            character.ObtainMoney();
            CurrentGame.Map.Entities.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
