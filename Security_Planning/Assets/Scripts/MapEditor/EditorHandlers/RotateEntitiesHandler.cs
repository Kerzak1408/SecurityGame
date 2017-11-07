using System.Linq;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class RotateEntitiesHandler : BaseUserSelectableHandler
    {
        private GameObject rotationArrow;
        private GameObject rotatedEntity;

        public RotateEntitiesHandler(GridManager gridManager) : base(gridManager)
        {
        }

        public override void Start()
        {
            if (rotationArrow == null)
            {
                rotationArrow =
                    gridManager.InstantiateGameObject(Resources.Load<GameObject>("Prefabs/UI/RotationArrow"));
                rotationArrow.SetActive(false);
            }
            
        }

        public override void End()
        {
            rotationArrow.SetActive(false);
            rotatedEntity = null;
        }

        public override void LeftButtonUp(RaycastHit[] raycastHitsHits)
        {
            Map currentMap = gridManager.GetCurrentMap();
            RaycastHit entityHit =
                raycastHitsHits.FirstOrDefault(hit => currentMap.Entities.Contains(hit.transform.gameObject));
            if (!entityHit.Equals(default(RaycastHit)))
            {
                rotatedEntity = entityHit.transform.gameObject;
                rotationArrow.SetActive(true);
                Vector3 rotatedPosition = entityHit.transform.position;
                rotationArrow.transform.position = new Vector3(rotatedPosition.x, rotatedPosition.y,
                    rotationArrow.transform.position.z);
            }
            else
            {
                rotationArrow.SetActive(false);
            }
        }

        public override void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed)
        {
            if (rotatedEntity != null)
            {
                float rotationSpeed = 45 * Time.deltaTime;
                if (keyCodesPressed.Contains(KeyCode.LeftArrow))
                {
                    rotatedEntity.transform.Rotate(0,0, rotationSpeed, Space.World);
                    gridManager.FlagCurrentButton();
                }
                if (keyCodesPressed.Contains(KeyCode.RightArrow))
                {
                    rotatedEntity.transform.Rotate(0, 0, -rotationSpeed, Space.World);
                    gridManager.FlagCurrentButton();
                }
            }
            base.PressedKeys(keyCodesUp, keyCodesDown, keyCodesPressed);
        }
    }
}
