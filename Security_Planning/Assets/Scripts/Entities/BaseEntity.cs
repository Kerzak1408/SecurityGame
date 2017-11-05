using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public abstract string PrefabName { get; set; }

    public virtual void StartGame() { }
    public abstract void Deserialize(BaseEntityData deserializedData);
    public abstract BaseEntityData Serialize();
}
