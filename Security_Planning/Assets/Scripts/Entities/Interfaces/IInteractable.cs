using System;
using Assets.Scripts.Entities.Characters;

namespace Assets.Scripts.Entities.Interfaces
{
    public interface IInteractable
    {
        void Interact(BaseCharacter character, Action success = null);
    }
}
