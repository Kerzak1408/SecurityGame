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
        private Ray ray;

        public override Type GetReceiverType()
        {
            return typeof(PIRAlarm);
        }

        public override void StartGame()
        {
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(currentPosition.x, currentPosition.y, -1f);
            camera = GetComponentInChildren<Camera>();
            camera.enabled = false;
            ray = new Ray();
            ray.origin = camera.transform.position;


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
                // Detect only entities that are PIR-Detetectable and lay in view of the PIR camera.
                if (entity.HasScriptOfType(typeof(IPIRDetectable)) && GeometryUtility.TestPlanesAABB(planes, entity.GetComponent<Collider>().bounds))
                {
                    // Aim to the center of the collider
                    Vector3 entityColliderCenter = entity.GetComponent<Collider>().bounds.center;
                    // Begin ray at the PIR camera
                    ray.origin = camera.transform.position;
                    ray.direction = entityColliderCenter - camera.transform.position;
                    RaycastHit hit;
                    // If the first hit is the entity, start alarm -> We have an entity inside PIR camera view + 
                    // nothing is between the entity and the PIR
                    if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == entity)
                    {
                        activate = true;
                    }
                }

                //if (entity.HasScriptOfType(typeof(IPIRDetectable)) &&
                //    GeometryUtility.TestPlanesAABB(planes, entity.GetComponent<Collider>().bounds))
                //{
                //    Debug.Log(entity.name + " has been detected!");
                //    activate = true;
                //}   
            }
            if (alarm != null)
            {
                alarm.Active = activate;
            }

        }
    }
}
