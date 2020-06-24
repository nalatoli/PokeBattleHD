using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides access to scenes cameras and offers high-levels control.
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary> Control cameras in this scene. </summary>
    public static CameraManager instance;

    [Header("References")]
    [Tooltip("Cameras in scene. The first one assigned will be used at the start of play.")]
    public AnimatedCamera[] animatedCameras;

    /* Private Properties */
    private AnimatedCamera mainCam;

    void Awake()
    {
        /* Initialize Instance (Throw Error If More Than One) */
        if (instance != null)
            Debug.LogWarning("More than one Camera Manager in this scene.");
        else
            instance = this;
    }

    void Start()
    {
        /* For All Cameras */
        foreach (AnimatedCamera animtedCam in animatedCameras)
        {
            /* If Main Camera NOT Assigned */
            if (mainCam == null)
            {
                /* Assign Main Camera and Enable This Camera */
                mainCam = animtedCam;
                mainCam.cam = mainCam.animControl.GetComponent<Camera>();
                mainCam.cam.enabled = true;
            }

            /* Else, Disable This Camera */
            else {
                animtedCam.cam = animtedCam.animControl.GetComponent<Camera>();
                animtedCam.cam.enabled = false;
            }

        }          
    }

    /// <summary> Play single animation on a single camera that takes up entire screen. </summary>
    /// <param name="camAnim"> Camera animation to be played. </param>
    public void PlayClip(AnimationClip clip, Transform target)
    {
        /* Restore Viewport of Main Camera */
        mainCam.cam.rect = new Rect(0, 0, 1, 1);

        /* Disable All Other Cameras */
        for (int i = 1; i < animatedCameras.Length; i++)
            animatedCameras[i].cam.enabled = false;

        /* Play Animation On Main Cam */
        mainCam.animControl.PlayClip(target, clip);     
    }

    public void PlayAnimation(Collider target, CameraAnimation camAnim)
    {
        mainCam.animControl.PlayAnimation(target, camAnim);
    }

    public void PlaySplitScreen(AnimationClip clip1, Transform target1, Rect rect1, AnimationClip clip2, Transform target2, Rect rect2)
    {
        /* Throw Error At User If Trying To Use Split Screen With A Lack Of Cameras */
        if(animatedCameras.Length < 2) {
            Debug.LogError("Cannot play split screen animations. Need at least 2 cameras in the camera manager.");
            return;
        }

        /* Enable 2nd Camera */
        animatedCameras[1].cam.enabled = true;

        /* Set The Viewports Of Two Cameras */
        animatedCameras[0].cam.rect = rect1;
        animatedCameras[1].cam.rect = rect2;

        /* Play The two animations */
        animatedCameras[0].animControl.PlayClip(target1, clip1);
        animatedCameras[1].animControl.PlayClip(target2, clip2);
    }

    /// <summary> Pans around specific target.</summary>
    /// <param name="target"> Collider target of animation. The camera will pan around this target. </param>
    /// <param name="distanceOffset"> Distance offset from target. 0 is no additional offset. (+) is further away. </param>
    /// <param name="verticalOffset"> Vertical offset from target. 0 is no additional offset. (+) is further up. </param>
    /// <param name="speed"> Speed of movement. Can be negative for reverse direction. </param>
    /// <param name="timeOffset"> Normalized Time in animation to start, from [0,1]. 0 is facing the back of target and 0.5 is facing the front. </param>
    public void Pan(Collider target, float distanceOffset, float verticalOffset, float speed, float timeOffset)
    {
        mainCam.animControl.PanAround(target, distanceOffset, verticalOffset, speed, timeOffset);
    }

    /// <summary> Pans around specific target. Automatically assigns some paramters. </summary>
    /// <param name="target"> Collider target of animation. The camera will pan around this target. </param>
    public void Pan(Collider target)
    {
        mainCam.animControl.PanAround(target, 0, Random.Range(0, 0.5f), 0.8f, Random.Range(0f, 0.2f));
    }

    /// <summary> Pans around specific target. Automatically assigns some paramters. </summary>
    /// <param name="target"> Collider target of animation. The camera will pan around this target. </param>
    /// <param name="reverseSpeed"> Determines if automatically assigned speed is reversed. </param>
    public void Pan(Collider target, bool reverseSpeed)
    {
        mainCam.animControl.PanAround(target, 0, Random.Range(0, 0.5f), 0.8f * (reverseSpeed ? -1 : 1), Random.Range(0.5f, 0.65f));
    }

    /// <summary> Focuses on target. Automatically assigns some paramters. </summary>
    /// <param name="target"> Collider target of animation. The camera will focus on this target. </param>
    public void Focus(Collider target)
    {
        mainCam.animControl.PanAround(target, Random.Range(-0.1f, 0.4f), Random.Range(-0.1f, 0.1f), 0.1f, Random.Range(0f, 0.15f));
    }


    /// <summary> Focuses on target.</summary>
    /// <param name="target"> Collider target of animation. The camera will focuses on this target. </param>
    /// <param name="distanceOffset"> Distance offset from target. 0 is no additional offset. (+) is further away. </param>
    /// <param name="verticalOffset"> Vertical offset from target. 0 is no additional offset. (+) is further up. </param>
    /// <param name="timeOffset"> Normalized Time in animation to start, from [0,1]. 0 is facing the back of target and 0.5 is facing the front. </param>
    public void Focus(Collider target, float distanceOffset, float verticalOffset, float timeOffset)
    {
        mainCam.animControl.PanAround(target, distanceOffset, verticalOffset, 0.1f, timeOffset);
    }

    [System.Serializable]
    public class AnimatedCamera
    {
        [HideInInspector]
        public Camera cam;
        public CameraAnimationControl animControl;
    }

}


