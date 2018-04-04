using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Entities.Characters.Behaviours;
using UnityEngine;
using UnityEngine.AI;

public class Burglar : BaseCharacter
{

    private float probability;
    private Vector3 directionVector;
    private CharacterController controller;

    private bool isPaused = true;
    private BaseBehaviour behaviour;

    // Use this for initialization
    public override void StartGame()
    {
        behaviour = new CollectEverythingBehaviour(this);
        behaviour.Start();
    }

	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
        IsMoving = false;
        if (isPaused || CurrentGame.IsFinished) return;
        behaviour.Update();
    }

    public void ChangePausedState()
    {
        isPaused = !isPaused;
    }

    public override void RequestPassword(IPasswordOpenable passwordOpenableObject)
    {
        throw new System.NotImplementedException();
    }

    public override void InterruptRequestPassword()
    {
        throw new System.NotImplementedException();
    }

    public override void RequestCard(CardReaderEntity cardReader)
    {
        cardReader.VerifyCard();
    }

    public override void Log(string line)
    {
        CurrentGame.Log("Burglar: " + line);
    }
    
}
