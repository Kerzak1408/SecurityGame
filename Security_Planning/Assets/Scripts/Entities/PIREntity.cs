using System;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class PIREntity : DetectorEntity
    {
        private GameObject relatedObject;
        private PIRAlarm alarm;
        private Ray ray;

        public override PlanningEdgeType PlanningEdgeType
        {
            get { return PlanningEdgeType.PIR; }
        }

        public override DetectorType DetectorType
        {
            get { return DetectorType.PIR; }
        }

        public override Type GetReceiverType()
        {
            return typeof(PIRAlarm);
        }

        public override void StartGame()
        {
            base.StartGame();
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(currentPosition.x, currentPosition.y, -1f);
            Camera.enabled = false;
            ray = new Ray();
            ray.origin = Camera.transform.position;

            relatedObject = GameObject.Find(Data.RelatedName);
            if (relatedObject != null)
            {
                alarm = relatedObject.GetComponent<PIRAlarm>();
            }
        }

        public void Update()
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera);
            bool activate = false;
            foreach (GameObject entity in CurrentGame.Map.Entities)
            {
                // Detect only entities that are PIR-Detetectable and lay in view of the PIR camera.
                if (entity.HasScriptOfType(typeof(IPIRDetectable)) && GeometryUtility.TestPlanesAABB(planes, entity.GetComponent<Collider>().bounds))
                {
                    // Aim to the center of the collider
                    Vector3 entityColliderCenter = entity.GetComponent<Collider>().bounds.center;
                    // Begin ray at the PIR camera
                    ray.origin = Camera.transform.position;
                    ray.direction = entityColliderCenter - Camera.transform.position;
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

        public override Vector3 GetEditorForward()
        {
            return -transform.up;
        }
    }
}
