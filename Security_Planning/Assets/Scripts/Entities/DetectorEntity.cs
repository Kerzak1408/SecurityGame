﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Assets.Scripts.Entities.Casting;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities
{   
    public abstract class DetectorEntity : TransmitterEntity, IInteractable, IPlanningEdgeCreator
    {
        private new Camera camera;

        protected Camera Camera
        {
            get { return camera ?? (camera = GetComponentInChildren<Camera>()); }
        }
        
        public abstract PlanningEdgeType PlanningEdgeType { get; }
        public abstract DetectorType DetectorType { get; }
        public GameObject Interactable { get; private set; }
        public float InteractTime
        {
            get { return Constants.Constants.DESTROY_TIME; }
        }

        public override void StartGame()
        {
            base.StartGame();
            Interactable = gameObject;
        }

        public bool ShouldExplore(PlanningNode node)
        {
            return !node.DestroyedDetectors.Contains(this);
        }

        public void ModifyNextNode(PlanningNode node)
        {
            node.DestroyedDetectors.Add(this);
        }

        public void MarkDetectableNodes(AIModel aiModel, GameObject[,] grid)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(Camera);
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    GameObject gridTile = grid[i, j];
                    if (gridTile == null) continue;
                    var collider = gridTile.GetComponent<Collider>();
                    //Vector3 viewportPoint = Camera.WorldToViewportPoint(gridTile.transform.position);
                    if (GeometryUtility.TestPlanesAABB(planes, collider.bounds))
                    {
                        Vector3 cameraPosition = Camera.transform.position;
                        Vector3 aboveGridPosition = gridTile.transform.position;
                        aboveGridPosition.y = 0.5f;
                        Ray ray = new Ray(cameraPosition, aboveGridPosition - cameraPosition);
                        RaycastHit[] raycastHits = Physics.RaycastAll(ray);
                        // Change to distance from the closest point of collider
                        float distance = Vector3.Distance(collider.ClosestPointOnBounds(cameraPosition), cameraPosition);
                        if (!raycastHits.Any(hit => hit.distance < distance && !hit.transform.gameObject.HasScriptOfType<BaseEntity>() &&
                                                    !hit.transform.gameObject.HasScriptOfType<BaseGate>()))
                        {
                            aiModel.Tiles[i, j].AddDetector(this);
                        }
                    }
                }
        }
        public void Interact(BaseCharacter character, Action success = null)
        {
            character.AttackForSeconds(InteractTime);
            Action wrapperSuccess = () =>
            {
                if (success != null)
                {
                    success();
                }
                Map.RemoveDetectorEntity(gameObject);
                Destroy(gameObject);
            };
            CastManager.Instance.Cast(character, Constants.Constants.DESTROY_TIME, null, wrapperSuccess);
        }

        public override string ToString()
        {
            return Interactable.name;
        }
    }
}
