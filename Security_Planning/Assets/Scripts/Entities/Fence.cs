using System.Collections;
using System.Threading;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Fence : MonoBehaviour, IInteractable
    {
        private float destroyCounter;
        private bool destroy;
        private AudioSource audioSource;

        public void Interact(BaseCharacter character)
        {
            if (character.GetActiveItem().HasScriptOfType<WireCutterItem>())
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.Play();
                destroy = true;
            }
        }

        private void Update()
        {
            if (destroy)
            {
                if (destroyCounter > audioSource.clip.length)
                {
                    Destroy(transform.gameObject);
                }
                else
                {
                    destroyCounter += Time.deltaTime;
                }
            }
        }
    }
}
