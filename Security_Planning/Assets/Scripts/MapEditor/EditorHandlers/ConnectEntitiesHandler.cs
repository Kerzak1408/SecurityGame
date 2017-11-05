using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    /// <summary>
    /// Handles connecting transmitters <see cref="TransmitterEntity"/> with the receivers <see cref="ReceiverEntity"/>
    /// </summary>
    public class ConnectEntitiesHandler : BaseUserSelectableHandler
    {
        private TransmitterEntity selectedTransmitter;
        private Map currentMap;

        public ConnectEntitiesHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void Start()
        {
            currentMap = gridManager.GetCurrentMap();
            currentMap.DeactivateEntitiesExceptOfType<TransmitterEntity>();
        }

        public override void End()
        {
            currentMap.ActivateAllEntities();
        }

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            if (selectedTransmitter == null)
            {
                RaycastHit transmitterHit =
                    raycastHits.FirstOrDefault(hit =>
                        hit.transform.gameObject.HasScriptOfType<TransmitterEntity>());
                if (!transmitterHit.Equals(default(RaycastHit)))
                {
                    selectedTransmitter = transmitterHit.transform.gameObject.GetComponent<TransmitterEntity>();
                    currentMap.DeactivateEntitiesExceptOfType<ReceiverEntity>();
                }
            }
            else
            {
                RaycastHit receiverHit = raycastHits.FirstOrDefault(hit =>
                    hit.transform.gameObject.HasScriptOfType<ReceiverEntity>());
                if (!receiverHit.Equals(default(RaycastHit)))
                {
                    selectedTransmitter.Data.relatedName = receiverHit.transform.name;
                    currentMap.ActivateAllEntities();
                    selectedTransmitter = null;
                    gridManager.FlagCurrentButton();
                }
            }
        }
    }
}
