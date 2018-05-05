using System;
using System.Collections;
using System.Threading;
using Assets.Scripts.Entities.Casting;
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
        private float cuttingLength;

        public EdgeType EdgeType
        {
            get { return EdgeType.FENCE; }
        }

        public IInteractable InteractableObject
        {
            get { return this; }
        }

        public bool IsOpen { get; private set; }

        public float DelayTime
        {
            get
            {
                return 0;
                return IsOpen ? 0 : cuttingLength;
            }
        }

        public float InteractTime
        {
            get { return DelayTime; }
        }

        public override void StartGame()
        {
            base.StartGame();
            audioSource = gameObject.AttachAudioSource("CuttingWire");
            cuttingLength = audioSource.clip.length;
        }

        public void Open(BaseCharacter character)
        {
            character.ActivateItem<WireCutterItem>();
            Interact(character, () => IsOpen = true);
        }

        public void Interact(BaseCharacter character, Action success = null)
        {
            if (character.GetActiveItem().HasScriptOfType<WireCutterItem>())
            {
                audioSource.Play();
                Action wrapperSuccesAction = () =>
                {
                    if (success != null)
                    {
                        success();
                    }

                    Destroy(gameObject);
                    foreach (Transform sibling in transform.parent)
                    {
                        GameObject siblingObject = sibling.gameObject;
                        if (siblingObject.HasScriptOfType<Fence>())
                        {
                            Destroy(siblingObject);
                        }
                    }
                };
                Action interruptAction = () => audioSource.Stop();
                CastManager.Instance.Cast(character, audioSource.clip.length, interruptAction, wrapperSuccesAction);
                
            }
        }
    }
}
