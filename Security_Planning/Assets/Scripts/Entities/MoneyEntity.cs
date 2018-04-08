using System;
using System.Collections;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class MoneyEntity : BaseEntityWithBaseData, IInteractable
    {
        private AudioSource moneyAudio;
        private float castLength;

        public override void StartGame()
        {
            base.StartGame();
            moneyAudio = gameObject.AttachAudioSource("Money", pitch:10);
            castLength = moneyAudio.clip.length;
        }

        public void Interact(BaseCharacter character, Action success = null)
        {
            moneyAudio.Play();
            Action wrapperSuccessAction = () =>
            {
                character.ObtainMoney();
                Map.Entities.Remove(gameObject);
                if (success != null)
                {
                    success();
                }

                Destroy(gameObject);
            };
            Action interruptAction = () => moneyAudio.Stop();
            float castTime = moneyAudio.clip.length / moneyAudio.pitch;
            CastManager.Instance.Cast(character, castTime, interruptAction, wrapperSuccessAction);
        }

        public float InteractTime
        {
            get { return castLength; }
        }
    }
}
