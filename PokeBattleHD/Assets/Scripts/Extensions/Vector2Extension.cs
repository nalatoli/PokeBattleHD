using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************
 * Extends Vector classes
 * 
 ************************************************************************/
public static class Vector2Extension
{
    /// <summary> Limits vector2 component-wise </summary>
    /// <param name="vec"> Vector to limit. </param>
    /// <param name="min"> Minimum values. </param>
    /// <param name="max"> Maximum values. </param>
    public static Vector2 Clamp(this Vector2 vec, Vector2 min, Vector2 max)
    {
        vec.x = vec.x < min.x ? min.x : (vec.x > max.x ? max.x : vec.x);
        vec.y = vec.y < min.y ? min.y : (vec.y > max.y ? max.y : vec.y);

        return vec;
    }

    /// <summary> Divides two vectors component-wise. </summary>
    /// <param name="b"> Numerator. </param>
    /// <param name="a"> Denominator. </param>
    /// <returns>Vector3 component-wise quotient. </returns>
    public static Vector3 DividedBy(this Vector3 b, Vector3 a)
    {
        return new Vector3(b.x / a.x, b.y / a.y, b.z / a.z);
    }
}
