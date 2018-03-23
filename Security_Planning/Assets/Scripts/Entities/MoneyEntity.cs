﻿using System;
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

        protected override void Start()
        {
            base.Start();
            moneyAudio = gameObject.AttachAudioSource("Money", pitch:10);
        }

        public void Interact(BaseCharacter character, Action successAction = null)
        {
            moneyAudio.Play();
            Action wrapperSuccessAction = () =>
            {
                character.ObtainMoney();
                CurrentGame.Map.Entities.Remove(gameObject);
                if (successAction != null)
                {
                    successAction();
                }

                Destroy(gameObject);
            };
            Action interruptAction = () => moneyAudio.Stop();
            float castTime = moneyAudio.clip.length / moneyAudio.pitch;
            CastManager.Instance.Cast(character, castTime, interruptAction, wrapperSuccessAction);
        }
    }
}
