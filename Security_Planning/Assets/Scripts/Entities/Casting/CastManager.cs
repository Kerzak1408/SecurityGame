using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Entities.Casting
{
    /// <summary>
    /// Singleton that manages the cast actions of the all characters.
    /// </summary>
    public class CastManager : MonoBehaviour
    {
        public static CastManager Instance { get; set; }

        /// <summary>
        /// Just one because we can only visualize the action of the one character at the time. 
        /// </summary>
        public Slider ProgressBar;
        /// Currently cast action by the given character.
        private Dictionary<BaseCharacter, CastAction> characterDictionary;
        private CastAction visualizedAction;

        public void Cast(BaseCharacter character, float castTime, Action interruptAction, Action successAction)
        {
            var castAction = new CastAction(castTime, interruptAction, successAction);
            if (characterDictionary.ContainsKey(character))
            {
                CastAction charactersAction = characterDictionary[character];
                charactersAction.Interrupt();
            }

            characterDictionary[character] = castAction;
            castAction.StartCast();
            if (character.IsActive)
            {
                visualizedAction = castAction;
                ProgressBar.gameObject.SetActive(true);
            }
            character.Cast(castAction);
        }

        public void CastingFinished(CastAction action)
        {
            KeyValuePair<BaseCharacter, CastAction> dictionaryPair = 
                characterDictionary.FirstOrDefault(kvPair => kvPair.Value == action);
            BaseCharacter castingCharacter = dictionaryPair.Key;
            castingCharacter.FinishCasting();
            if (!default(KeyValuePair<BaseCharacter, CastAction>).Equals(dictionaryPair))
            {
                characterDictionary.Remove(castingCharacter);
            }

            if (visualizedAction == action)
            {
                ProgressBar.gameObject.SetActive(false);
                visualizedAction = null;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                characterDictionary = new Dictionary<BaseCharacter, CastAction>();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            var actions = new List<CastAction>(characterDictionary.Values);
            foreach (CastAction castAction in actions)
            {
                castAction.Update();
            }

            if (visualizedAction != null)
            {
                ProgressBar.value = 1 - visualizedAction.TimeLeft / visualizedAction.CastTime;
            }
        }
    }
}
