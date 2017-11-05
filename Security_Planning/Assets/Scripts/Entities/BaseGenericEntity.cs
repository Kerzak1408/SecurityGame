using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGenericEntity<T> : BaseEntity where T : BaseEntityData, new()
{
    private T data;
    public T Data
    {
        get
        {
            if (data == null)
            {
                data = new T();
            }
            return data;
        }
        set
        {
            data = value;
        }
    }

    public override string PrefabName
    {
        get { return Data.prefabName; }
        set { Data.prefabName = value; }
    }

    public override BaseEntityData Serialize()
    {
        Data.ExtractValuesFromGameObject(transform.gameObject);
        return Data;
    }

    public override void Deserialize(BaseEntityData deserializedData)
    {
        transform.position = deserializedData.position;
        transform.eulerAngles = deserializedData.eulerAngles;
        name = deserializedData.name;
        Data = (T)deserializedData;
    }
}
