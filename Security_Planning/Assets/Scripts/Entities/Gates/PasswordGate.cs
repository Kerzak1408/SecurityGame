using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
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

        protected void OnTriggerStay(Collider other)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            if (allowOpening)
            {
                Open();
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
}
