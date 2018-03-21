using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Window : MonoBehaviour, IInteractable, IObstacle
    {
        private int crackState;
        private int maxCrack;
        private GameObject crackObject;
        private AudioSource glassBreak1;
        private AudioSource glassBreak2;
        private bool destroy;
        private float destroyCounter;

        public EdgeType EdgeType
        {
            get { return EdgeType.WINDOW; }
        }

        public IInteractable InteractableObject
        {
            get { return this; }
        }

        private void Start()
        {
            crackState = 0;
            maxCrack = 3;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            glassBreak1 = audioSources.First(audio => audio.clip.name == "GlassBreak1");
            glassBreak2 = audioSources.First(audio => audio.clip.name == "GlassBreak2");
        }

        private void Update()
        {
            if (destroy)
            {
                if (destroyCounter > glassBreak2.clip.length)
                {
                    Destroy(gameObject);
                    foreach (Transform sibling in transform.parent)
                    {
                        if (sibling.name.Substring(0, 5).Equals("Crack"))
                        {
                            Destroy(sibling.gameObject);
                        }
                    }
                }
                else
                {
                    destroyCounter += Time.deltaTime;
                }
            }
        }

        public void Interact(BaseCharacter character, Action successAction = null)
        {
            character.Attack();
            
            if (crackObject != null)
            {
                crackObject.SetActive(false);
            }
            if (++crackState > maxCrack)
            {
                glassBreak2.Play();
                destroy = true;
            }
            else
            {
                glassBreak1.Play();
                Transform findChild = transform.parent.Find("Crack" + crackState);
                if (!findChild.Equals(default(Transform)))
                {
                    findChild.gameObject.SetActive(true);
                }
            }
        }

        private IEnumerator PlaySound()
        {
            GetComponent<AudioSource>().Play();
            yield return null;
        }
    }
}
