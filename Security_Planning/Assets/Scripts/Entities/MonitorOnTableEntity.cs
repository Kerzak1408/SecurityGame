using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class MonitorOnTableEntity : MonitorEntity
    {
        public override void StartGame()
        {
            base.StartGame();
            transform.position += 0.5f * Vector3.back;
        }
    }
}
