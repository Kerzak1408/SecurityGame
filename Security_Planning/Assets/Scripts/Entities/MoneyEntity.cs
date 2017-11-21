using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class MoneyEntity : BaseEntityWithBaseData, IInteractable
    {
        private AudioSource moneyAudio;

        protected override void Start()
        {
            base.Start();
            moneyAudio = gameObject.AttachAudioSource("Money");
        }

        public void Interact(BaseCharacter character)
        {
            moneyAudio.Play();
            character.ObtainMoney();
            CurrentGame.Map.Entities.Remove(gameObject);
            DestroyAfterTimeout(moneyAudio.clip.length);
        }
    }
}
