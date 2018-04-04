using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using UnityEngine;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public class RotateEntitiesHandler : BaseUserSelectableHandler
    {
        private GameObject rotationArrow;
        private GameObject rotatedEntity;
        private float rotateBack;

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

        public override void LeftButtonUp(RaycastHit[] raycastHits)
        {
            Map currentMap = gridManager.GetCurrentMap();
            RaycastHit entityHit =
                raycastHits.FirstOrDefault(hit => currentMap.Entities.Contains(hit.transform.gameObject));
            if (!entityHit.Equals(default(RaycastHit)))
            {
                rotatedEntity = entityHit.transform.gameObject;
                rotationArrow.SetActive(true);
                AdjustArrowRotation();
                Vector3 rotatedPosition = entityHit.transform.position;
                rotationArrow.transform.position = new Vector3(rotatedPosition.x, rotatedPosition.y,
                    rotationArrow.transform.position.z);
            }
            else
            {
                rotatedEntity = null;
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
                    RotateEntity(rotationSpeed);
                }
                if (keyCodesPressed.Contains(KeyCode.RightArrow))
                {
                    RotateEntity(-rotationSpeed);
                }
            }
            base.PressedKeys(keyCodesUp, keyCodesDown, keyCodesPressed);
        }

        private void RotateEntity(float rotationSpeed)
        {
            rotatedEntity.transform.Rotate(0, 0, rotationSpeed, Space.World);
            rotationArrow.transform.Rotate(0, 0, rotationSpeed, Space.World);
            gridManager.FlagCurrentButton();
            AdjustArrowRotation();
        }

        private void AdjustArrowRotation()
        {
            Vector3 editorForward = rotatedEntity.GetComponent<BaseEntity>().GetEditorForward();

            LineRenderer lineRenderer = rotationArrow.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            Vector3 position = new Vector3(rotatedEntity.transform.position.x, rotatedEntity.transform.position.y, -10);
            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, position + editorForward.normalized * 0.5f);
        }
    }
}
