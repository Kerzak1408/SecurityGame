using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PasswordGate : GateOpen, IPasswordOpenable
{
    [SerializeField]
    private string password = "1234";
    private bool allowOpening;
    public string Password { get { return password; } set { password = value; } }
    

    public void EnterPassword(string password, BaseCharacter character)
    {
        if (Password == password)
        {
            allowOpening = true;
            base.OnTriggerEnter(null);
            character.InterruptRequestPassword();
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        var character = other.gameObject.GetComponent<BaseCharacter>();
        if (character != null)
        {
            character.RequestPassword(this);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        if (allowOpening)
        {
            base.OnTriggerStay(other);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        var character = other.gameObject.GetComponent<BaseCharacter>();
        character.InterruptRequestPassword();
        allowOpening = false;
        base.OnTriggerExit(other);
    }
}
