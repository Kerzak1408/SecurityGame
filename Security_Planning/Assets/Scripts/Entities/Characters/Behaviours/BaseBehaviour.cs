using Assets.Scripts.Entities.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities.Characters.Behaviours
{
    public abstract class BaseBehaviour
    {
        protected BaseCharacter character;
        public bool IsInitialized { get; protected set; }

        public BaseBehaviour(BaseCharacter character)
        {
            this.character = character;
        }

        public abstract void Start();

        public abstract void Update();
    }
}

