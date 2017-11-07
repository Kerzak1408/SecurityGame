using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class ReceiverEntity : BaseEntityWithBaseData
    {
        public override void StartGame()
        {
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -0.8f);
        }
    }
}
