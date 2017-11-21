using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class Hedge : BaseObject
    {
        protected override void Start()
        {
            base.Start();
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -1);
        }
    }
}
