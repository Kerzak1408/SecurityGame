using System;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class PIREntity : TransmitterEntity
    {
        private Camera camera;
        private Plane[] planes;
        private GameObject relatedObject;
        private PIRAlarm alarm;

        public override Type GetReceiverType()
        {
            return typeof(PIRAlarm);
        }

        public override void StartGame()
        {
            camera = GetComponent<Camera>();
            camera.enabled = false;
            planes = GeometryUtility.CalculateFrustumPlanes(camera);
            relatedObject = GameObject.Find(Data.relatedName);
            if (relatedObject != null)
            {
                alarm = relatedObject.GetComponent<PIRAlarm>();
            }
        }

        public void Update()
        {
            bool activate = false;
            foreach (GameObject entity in CurrentGame.Map.Entities)
            {
                if (entity.HasScriptOfType(typeof(IPIRDetectable)) &&
                    GeometryUtility.TestPlanesAABB(planes, entity.GetComponent<Collider>().bounds))
                {
                    Debug.Log(entity.name + " has been detected!");
                    activate = true;
                }   
            }
            if (alarm != null)
            {
                alarm.Active = activate;
            }

        }
    }
}
