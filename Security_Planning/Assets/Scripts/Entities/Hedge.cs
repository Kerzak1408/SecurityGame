using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Hedge : BaseObject
    {
        public override void StartGame()
        {
            base.StartGame();
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -1);
        }
    }
}
