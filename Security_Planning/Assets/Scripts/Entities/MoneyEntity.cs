using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Entities
{
    public class MoneyEntity : BaseEntityWithBaseData, IInteractable
    {
        public void Interact(BaseCharacter character)
        {
            character.ObtainMoney();
            CurrentGame.Map.Entities.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
