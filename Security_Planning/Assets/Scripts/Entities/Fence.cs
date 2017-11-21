using System.Collections;
using System.Threading;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Fence : BaseObject, IInteractable
    {
        private AudioSource audioSource;

        protected override void Start()
        {
            base.Start();
            audioSource = gameObject.AttachAudioSource("CuttingWire");
        }

        public void Interact(BaseCharacter character)
        {
            if (character.GetActiveItem().HasScriptOfType<WireCutterItem>())
            {
                audioSource.Play();
                DestroyAfterTimeout(audioSource.clip.length);
            }
        }
    }
}
