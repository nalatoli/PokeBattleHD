using System.Collections;
using UnityEngine;

public class BeamEmissionLookAt : MonoBehaviour
{

    
    /// <summary> Lock on routine tracker. </summary>
    IEnumerator routine;

    /// <summary> Forces beam emitter to look at specified location. </summary>
    /// <param name="pos"> World position to look at. </param>
    public void EnableLockOn(Vector3 pos)
    {
        this.StartInterruptableCoroutine(LockOn(pos), ref routine);
        Debug.DrawLine(transform.position, pos, Color.blue, 2f);
    }

    /// <summary> Disables look at for beam emitter. </summary>
    public void DisableLockOn()
    {
        if (routine != null) StopCoroutine(routine);
    }

    /// <summary> Continuosly look at desiginated position. </summary>
    /// <param name="pos"> World position to look at. </param>
    /// <returns></returns>
    IEnumerator LockOn(Vector3 pos)
    {
        while(true) {
            transform.LookAt(pos);
            yield return null;
        }       
    }
}
