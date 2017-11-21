using System.Collections;
using System.Threading;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Window : MonoBehaviour, IInteractable
    {
        private int crackState;
        private int maxCrack;
        private GameObject crackObject;

        private void Start()
        {
            crackState = 0;
            maxCrack = 3;
        }

        public void Interact(BaseCharacter character)
        {
            character.Attack();

            AudioClip clip;
            if (crackObject != null)
            {
                crackObject.SetActive(false);
            }
            if (++crackState > maxCrack)
            {
                clip = Resources.Load<AudioClip>("Sounds/GlassBreak2");
                foreach (Transform sibling in transform.parent)
                {
                    if (sibling.name.Substring(0, 5).Equals("Crack"))
                    {
                        Destroy(sibling.gameObject);
                    }
                }
                Destroy(gameObject);
            }
            else
            {
                clip = Resources.Load<AudioClip>("Sounds/GlassBreak1");
                Transform findChild = transform.parent.Find("Crack" + crackState);
                if (!findChild.Equals(default(Transform)))
                {
                    findChild.gameObject.SetActive(true);
                }
            }
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }

        private IEnumerator PlaySound()
        {
            GetComponent<AudioSource>().Play();
            yield return null;
        }
    }
}
