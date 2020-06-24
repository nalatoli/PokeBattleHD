using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/************************************************************************
 * Extends Transform class
 * 
 ************************************************************************/
public static class TransformExtension
{
    /// <summary> Moves transform overtime to target position. </summary>
    /// <param name="transform"> Transform to move. </param>
    /// <param name="target"> Target to end up at. </param>
    /// <param name="speed"> Speed at which to get to the new position. </param>
    /// <param name="type"> Type of movement to use. </param>
    public static IEnumerator MoveOvertime(this Transform transform, Transform target, float speed, MoveOvertimeType type)
    {
        switch(type)
        {
            case MoveOvertimeType.Linear:

                while (transform.position != target.position) {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
                    yield return null;
                } break;

            case MoveOvertimeType.Smooth:

                while (transform.position != target.position) {
                    transform.position = Vector3.Slerp(transform.position, target.position, Time.deltaTime * speed);
                    yield return null;
                } break;

            default:
                Debug.Log("Invalide type for move transform: " + type);
                break;

        }
    }

    /// <summary> Moves transform overtime to target position. </summary>
    /// <param name="transform"> Transform to move. </param>
    /// <param name="target"> Target to end up at. </param>
    /// <param name="speed"> Speed at which to get to the new position. </param>
    /// <param name="type"> Type of movement to use. </param>
    public static IEnumerator MoveOvertime(this Transform transform, Vector3 target, float speed, MoveOvertimeType type)
    {
        switch(type)
        {
            case MoveOvertimeType.Linear:

                while (transform.position != target) {
                    transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
                    yield return null;
                } break;

            case MoveOvertimeType.Smooth:

                while (transform.position != target) {
                    transform.position = Vector3.Slerp(transform.position, target, Time.deltaTime * speed);
                    yield return null;
                } break;

            default:
                Debug.Log("Invalide type for move transform: " + type);
                break;

        }
    }

    /// <summary> Linearly scales transform overtime to target scale. </summary>
    /// <param name="transform"> Transform to resize. </param>
    /// <param name="targetScale"> Target scale to end up at. </param>
    /// <param name="speed"> Speed of scale. </param>
    public static IEnumerator ScaleOvertime(this Transform transform, Vector3 targetScale, float speed)
    {
        /* Lerp Overtime To Target Scale */
        while (transform.localScale != targetScale) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * speed);
            yield return null;
        }
    }

    /// <summary> Resets local position, rotation, and scale. </summary>
    /// <param name="transform"> Transform to resize. </param>
    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

}

public enum MoveOvertimeType
{
    /// <summary> Linearly interpolates between start and end position. </summary>
    Linear,

    /// <summary> Reaches end position slowly. </summary>
    Smooth
}
