using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector3Wrapper
{
    [SerializeField]
    private float x;
    [SerializeField]
    private float y;
    [SerializeField]
    private float z;

    public Vector3Wrapper(Vector3 value)
    {
        x = value.x;
        y = value.y;
        z = value.z;
    }

    public static implicit operator Vector3Wrapper(Vector3 value)
    {
        return new Vector3Wrapper(value);
    }

    public static implicit operator Vector3(Vector3Wrapper value)
    {
        return new Vector3(value.x, value.y, value.z);
    }
}
