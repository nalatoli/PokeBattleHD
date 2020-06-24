using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCameraAnimation", menuName = "Camera Animation")]
public class CameraAnimation : ScriptableObject
{
    [Tooltip("Translation offset of the camera. Z-Offset is distance from object.")]
    public Vector3 startOffset = Vector3.forward;

    [Tooltip("Angle offset to rotate camera.")]
    public Vector3 startAngle = new Vector3(-30, 10, 0);

    [Tooltip("Positional and rotational movement over time.")]
    public AnimationCurves3D animation;

    public IEnumerator Animate(Collider target, Camera cam, float m)
    {
        /* Set Wrap Mode For Curves */
        animation.Initialize();

        /* Initialize Recordings Of Camera */
        Vector3 originalCamPos = cam.transform.localPosition;
        Vector3 originalCamRot = cam.transform.localRotation.eulerAngles;
        Vector3 originalOffset = startOffset;
        Vector3 originalAngle = startAngle;
        float originalTimeOffset = animation.timeOffset;

        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Perform Motions Until Loop Ends (If It Ends) */
        while (elapsedTime <= animation.duration || animation.wrapMode != WrapMode.Once)
        {
            /* Update Time Offset From Editor */
            if(animation.timeOffset != originalTimeOffset) {
                elapsedTime = animation.timeOffset * animation.duration;
                originalTimeOffset = animation.timeOffset;
            }

            /* Update Position And Rotation at This Time */
            cam.transform.position =  originalCamPos + animation.EvaluateTranslation(elapsedTime, m);
            cam.transform.rotation =  Quaternion.Euler(originalCamRot + animation.EvaluateAngles(elapsedTime, m));

            /* Change Cam Offset In Editor At Runtime */
            if (startOffset != originalOffset) {
                originalCamPos += (new Vector3(
                    startOffset.x * m,
                    startOffset.y,
                    startOffset.z) 
                    - originalOffset);
                originalOffset = startOffset;
            }

            /* Change Cam Rotation In Editor At Runtime */
            if (startAngle != originalAngle) {
                originalCamRot += (new Vector3(
                    startAngle.x ,
                    startAngle.y * m,
                    startAngle.z)
                    - originalAngle);
                originalAngle = startAngle;
            }

            /* Update Time and Wait Until Next Frame */
            elapsedTime += Time.deltaTime;
            yield return null; 

        }
    }
            

    [System.Serializable]
    public class AnimationCurves3D
    {
        [Tooltip("Duration of entire animation. " +
            "Duration of 0 means no animation, but will set to start position and rotation." +
            "To loop animation, set wrap mode to loop.")]
        public float duration = 1;

        [Tooltip("Normalized time at which to start the curves"), Range(0,1)]
        public float timeOffset = 0;

        [Tooltip("Determines how curve is wrapped. Once means curve will play once and ping pong means curve will loop forwards and backwards.")]
        public WrapMode wrapMode = WrapMode.Loop;

        [Space]
        public AnimationCurve xOffset = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float xOffsetMultiplier = 1;

        [Space]
        public AnimationCurve yOffset = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float yOffsetMultiplier = 1;

        [Space]
        public AnimationCurve zOffset = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float zOffsetMultiplier = 1;

        [Space]
        public AnimationCurve xAngle = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float xAngleMultiplier = 1;

        [Space]
        public AnimationCurve yAngle = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float yAngleMultiplier = 1;

        [Space]
        public AnimationCurve zAngle = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        public float zAngleMultiplier = 1;


        /// <summary> Sets wrap mode of all animation curves. </summary>
        public void Initialize ()
        {
            xOffset.preWrapMode = wrapMode; xOffset.postWrapMode = wrapMode;
            yOffset.preWrapMode = wrapMode; yOffset.postWrapMode = wrapMode;
            zOffset.preWrapMode = wrapMode; zOffset.postWrapMode = wrapMode;
            xAngle.preWrapMode = wrapMode; xAngle.postWrapMode = wrapMode;
            yAngle.preWrapMode = wrapMode; yAngle.postWrapMode = wrapMode;
            zAngle.preWrapMode = wrapMode; zAngle.postWrapMode = wrapMode;
        }

        public Vector3 EvaluateTranslation(float t, float m)
        {
            /* Normalize Time */
            t /= duration;

            /* Return Evaluate Of Offset At Absolute Time */
            return new Vector3(
                xOffset.Evaluate(t) * xOffsetMultiplier * m, 
                yOffset.Evaluate(t) * yOffsetMultiplier, 
                zOffset.Evaluate(t) * zOffsetMultiplier);
        }

        public Vector3 EvaluateAngles(float t, float m)
        {
            /* Normalize Time */
            t /= duration;

            /* Return Evaluate Of Offset At Absolute Time */
            return new Vector3(
                xAngle.Evaluate(t) * xAngleMultiplier , 
                yAngle.Evaluate(t) * yAngleMultiplier * m,
                zAngle.Evaluate(t) * zAngleMultiplier);
        }

    }
}

