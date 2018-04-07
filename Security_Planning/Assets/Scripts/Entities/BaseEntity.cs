using Assets.Scripts.DataStructures;
using Assets.Scripts.MapEditor;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class BaseEntity : BaseObject
    {
        public abstract string PrefabName { get; set; }

        private Game currentGame;
        public Game CurrentGame
        {
            get { return currentGame; }
            set
            {
                currentGame = value;
                Map = currentGame.Map;
            }
        }

        public Map Map { get; set; }

        
        public abstract void Deserialize(BaseEntityData deserializedData);
        public abstract BaseEntityData Serialize();

        /// <summary>
        /// Get the real forward Vector of entity, where we consider it to be. 
        /// This method is needed because the forward of imported models is sometimes misleading.
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetEditorForward()
        {
            return transform.forward;
        }
    }
}
