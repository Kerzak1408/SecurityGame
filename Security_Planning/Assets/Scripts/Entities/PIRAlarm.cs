using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class PIRAlarm : ReceiverEntity
    {
        private float timeOfCurrentBlick;

        public bool Active { get; set; }

        public void Update()
        {
            if (Active)
            {
                timeOfCurrentBlick += Time.deltaTime;
                if (timeOfCurrentBlick > 0.5f)
                {
                    Color materialColor = GetComponent<Renderer>().material.color;
                    Color newColor = materialColor == Color.red ? Color.yellow : Color.red;
                    gameObject.ChangeColor(newColor);
                    timeOfCurrentBlick = 0;
                }
            }
            else
            {
                timeOfCurrentBlick = 0;
            }
        }
    }
}
