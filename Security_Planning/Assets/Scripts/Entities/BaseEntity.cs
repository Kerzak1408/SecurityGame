using Assets.Scripts.MapEditor;
using Assets.Scripts.Serialization;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public abstract class BaseEntity : MonoBehaviour
    {
        public abstract string PrefabName { get; set; }
        public Game CurrentGame { get; set; }

        protected virtual void Start() { }
        protected virtual void Update() { }

        public virtual void StartGame() { }
        public abstract void Deserialize(BaseEntityData deserializedData);
        public abstract BaseEntityData Serialize();
    }
}
