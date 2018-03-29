using System;
using System.Collections;
using System.Collections.Generic;
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
        private Dictionary<AudioSource, float> delays;

        public EdgeType EdgeType
        {
            get { return EdgeType.WINDOW; }
        }

        public IInteractable InteractableObject
        {
            get { return this; }
        }

        public bool IsOpen { get; private set; }
        public float DelayTime
        {
            get { return ComputeDelayTime(glassBreak1) + ComputeDelayTime(glassBreak2); }
        }

        private void Start()
        {
            crackState = 0;
            maxCrack = 3;
            AudioSource[] audioSources = GetComponents<AudioSource>();
            glassBreak1 = audioSources.First(audio => audio.clip.name == "GlassBreak1");
            glassBreak2 = audioSources.First(audio => audio.clip.name == "GlassBreak2");
            delays = new Dictionary<AudioSource, float>();
            delays[glassBreak1] = glassBreak1.clip.length;
            delays[glassBreak2] = glassBreak2.clip.length;
        }

        private float ComputeDelayTime(AudioSource audioSource)
        {
            return audioSource == null ? 0 : delays[audioSource];
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

                    IsOpen = true;
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

        public void Open(BaseCharacter character)
        {
            while (!destroy)
            {
                Interact(character);
            }
        }
    }
}
