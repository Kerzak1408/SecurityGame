using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class PIRAlarm : ReceiverEntity
    {
        private float timeOfCurrentBlick;
        private AudioSource alarmAudio;

        public bool Active { get; set; }

        public override void StartGame()
        {
            alarmAudio = GetComponent<AudioSource>();
            base.StartGame();
        }

        public void Update()
        {
            if (Active)
            {
                if (!alarmAudio.isPlaying)
                {
                    alarmAudio.Play();
                }
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
                alarmAudio.Stop();
                timeOfCurrentBlick = 0;
            }
        }
    }
}
