using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class MonitorEntity : ReceiverEntity
    {
        public override void StartGame()
        {
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -0.8f);
        }
    }
}
