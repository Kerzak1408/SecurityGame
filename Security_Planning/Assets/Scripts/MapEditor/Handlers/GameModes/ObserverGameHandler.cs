using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.GameModes
{
    public class ObserverGameHandler : BaseGameUserHandler
    {
        public override string Name
        {
            get { return "Observer"; }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Game.SwitchCamera();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Burglar.ChangePausedState();
            }
        }
    }
}
