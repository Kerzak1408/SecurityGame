using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    private Dictionary<string, object> data;
    protected Dictionary<string, object> Data
    {
        get
        {
            if (data == null)
            {
                data = new Dictionary<string, object>();
            }
            return data;
        }
        set
        {
            data = value;
        }
    }

    public string PrefabName { get; set; }

    private readonly string positionKey = "position";
    private readonly string nameKey = "name";

    public virtual Dictionary<string, object> Serialize()
    {
        Data[positionKey] = new Vector3Wrapper(transform.position);
        Data[nameKey] = name;
        return Data;
    }
    public virtual void Deserialize(Dictionary<string, object> deserializedData)
    {
        transform.position = (Vector3Wrapper) deserializedData[positionKey];
        name = (string) deserializedData[nameKey];
        Data = deserializedData;
    }
}
