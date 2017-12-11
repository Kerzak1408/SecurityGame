using System;
using UnityEngine;

public enum Axis {
    X, Y, Z
}

public static class AxisUtils
{
    public static Vector3 GetDirectionVector(this Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            case Axis.Z:return Vector3.forward;
        }
        throw new Exception("Unexpected axis type. ");
    }
}
