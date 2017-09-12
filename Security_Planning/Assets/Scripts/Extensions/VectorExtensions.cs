using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Gets the vector compoarison coordinate. Thew comparison coordinate is determined by the direction of the translation of the gate.
    /// For vertical gates return y coord, for horizontal gates x one.
    /// </summary>
    /// <param name="vector">The vector.</param>
    public static double GetVectorCoord(this Vector3 vector, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                {
                    return vector.x;
                }
            case Axis.Y:
                {
                    return vector.y;
                }
            case Axis.Z:
                {
                    return vector.z;
                }
        }
        return 0;
    }
}
