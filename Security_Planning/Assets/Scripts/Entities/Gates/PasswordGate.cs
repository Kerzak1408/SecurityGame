using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasswordGate : GateOpen, IPasswordOpenable
{
    private string password = "1234";
    private bool allowOpening;

    public void EnterPassword(string password, BaseCharacter character)
    {
        if (this.password == password)
        {
            allowOpening = true;
            base.OnTriggerEnter(null);
            character.InterruptRequestPassword();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<BaseCharacter>();
        if (character != null)
        {
            character.RequestPassword(this);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (allowOpening)
        {
            base.OnTriggerStay(other);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        var character = other.gameObject.GetComponent<BaseCharacter>();
        character.InterruptRequestPassword();
        allowOpening = false;
        base.OnTriggerExit(other);
    }
}
