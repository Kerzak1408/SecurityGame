using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.EditorHandlers
{
    public abstract class BaseEditorHandler : BaseHandler
    {
        protected GridManager gridManager;
        private string name;

        public override string Name
        {
            get
            {
                string className = GetType().Name;
                className = className.Substring(0, className.Length - 7);
                className = Regex.Replace(className, "(\\B[A-Z])", " $1");
                return className;
            }
        }

        protected BaseEditorHandler(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }

        public virtual void LeftButtonDown(RaycastHit[] raycastHits) { }
        public virtual void LeftButton(RaycastHit[] raycastHits) { }
        public virtual void LeftButtonUp(RaycastHit[] raycastHits) { }
        public virtual void Scroll(float scroll, RaycastHit[] raycastHits) { }
        public virtual void PressedKeys(KeyCode[] keyCodesUp, KeyCode[] keyCodesDown, KeyCode[] keyCodesPressed) { }

        public virtual void HoverLogic(RaycastHit[] raycastHits)
        {
            IEnumerable<RaycastHit> validObjects = raycastHits.Where(hit => !hit.transform.gameObject.Equals(gridManager.Panel) &&
                                                                           !hit.transform.gameObject.Equals(gridManager.PanelEntities) &&
                                                                           !hit.transform.gameObject.Equals(gridManager.PanelInfo));
            RaycastHit objectToChoose = default(RaycastHit);
            foreach (RaycastHit validObject in validObjects)
            {
                if (objectToChoose.Equals(default(RaycastHit)) ||
                    validObject.transform.position.z < objectToChoose.transform.position.z)
                {
                    objectToChoose = validObject;
                }
            }
            if (!objectToChoose.Equals(default(RaycastHit)))
            {
                string[] splitName = objectToChoose.transform.name.Split('_');
                string result = "";
                Regex regexDigitsOnly = new Regex(@"^\d+$");
                foreach (string namePart in splitName)
                {
                    if (!regexDigitsOnly.IsMatch(namePart))
                    {
                        result += namePart + " ";
                    }
                }
                result = Regex.Replace(result, "(\\B[A-Z])", " $1");
                gridManager.PanelInfo.GetComponentInChildren<Text>().text = result;
            }
        }

        public virtual void Start() { gridManager.DropdownMode.RefreshShownValue(); }
        public virtual void End() { }
    }
}
