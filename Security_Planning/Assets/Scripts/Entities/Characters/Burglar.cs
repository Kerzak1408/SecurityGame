using Assets.Scripts.Entities.Characters.Behaviours;
using Assets.Scripts.Entities.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters
{
    public class Burglar : BaseCharacter
    {
        private bool isPaused = true;
        public BaseBehaviour Behaviour;

        public bool IsInitialized
        {
            get { return Behaviour.IsInitialized; }
        }

        public bool IsPlanningStarted { get; private set; }

        // Use this for initialization
        public override void StartGame()
        {
            base.StartGame();
            Behaviour = new CollectEverythingBehaviour(this);
        }

        public void StartPlanning()
        {
            Behaviour.Start();
            IsPlanningStarted = true;
        }

        // Update is called once per frame
        protected override void UpdateGame()
        {
            base.UpdateGame();
            IsMoving = false;
            if (isPaused || CurrentGame.IsFinished || !Behaviour.IsInitialized) return;
            Behaviour.Update();
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
}
