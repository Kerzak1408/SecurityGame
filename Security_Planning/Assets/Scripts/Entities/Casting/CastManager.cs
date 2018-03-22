﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities.Characters;
using UnityEngine;
using UnityEngine.UI;

public class CastManager : MonoBehaviour
{
    public static CastManager Instance { get; set; }
    public Slider ProgressBar;

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
        if (!default(KeyValuePair<BaseCharacter, CastAction>).Equals(dictionaryPair))
        {
            characterDictionary.Remove(dictionaryPair.Key);
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
            DontDestroyOnLoad(gameObject);
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
        foreach (CastAction castAction in characterDictionary.Values)
        {
            castAction.Update();
        }

        if (visualizedAction != null)
        {
            ProgressBar.value = 1 - visualizedAction.TimeLeft / visualizedAction.CastTime;
        }
    }
}
