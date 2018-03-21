using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Fence : BaseObject, IInteractable, IObstacle
    {
        private AudioSource audioSource;

        public EdgeType EdgeType
        {
            get { return EdgeType.FENCE; }
        }

        public IInteractable InteractableObject
        {
            get { return this; }
        }

        protected override void Start()
        {
            base.Start();
            audioSource = gameObject.AttachAudioSource("CuttingWire");
        }

        public void Interact(BaseCharacter character, Action successAction = null)
        {
            if (character.GetActiveItem().HasScriptOfType<WireCutterItem>())
            {
                audioSource.Play();
                StartCoroutine(CallAfterTimeout(audioSource.clip.length, () =>
                {
                    if (successAction != null)
                    {
                        successAction();
                    }

                    Destroy(gameObject);
                }));
            }
        }
    }
}
