using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.MapEditor.Handlers.EditorHandlers
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
            currentMap = GridManager.GetCurrentMap();
            currentMap.DeactivateEntitiesExceptOfType(typeof(TransmitterEntity));
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
                    currentMap.DeactivateEntitiesExceptOfType(selectedTransmitter.GetReceiverType());
                }
            }
            else
            {
                RaycastHit receiverHit = raycastHits.FirstOrDefault(hit =>
                    hit.transform.gameObject.HasScriptOfType(selectedTransmitter.GetReceiverType()));
                if (!receiverHit.Equals(default(RaycastHit)))
                {
                    selectedTransmitter.Data.relatedName = receiverHit.transform.name;
                    currentMap.ActivateAllEntities();
                    selectedTransmitter = null;
                    GridManager.FlagCurrentButton();
                }
            }
        }
    }
}
