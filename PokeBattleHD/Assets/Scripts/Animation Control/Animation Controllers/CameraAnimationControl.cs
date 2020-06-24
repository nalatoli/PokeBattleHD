using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAnimationControl : ScriptedAnimationController
{
    /* Private Components */
    Camera cam;
    IEnumerator currentRoutine;

    /// <summary> Override controller that allows custom clips to be played. </summary>
    protected AnimatorOverrideController overrideController;

    protected override void Awake()
    {
        /* Get Components */
        base.Awake();
        cam = GetComponent<Camera>();
        overrideController = new AnimatorOverrideController(Animator.runtimeAnimatorController);
        Animator.runtimeAnimatorController = overrideController;
    }

    /// <summary> Plays 'clip', overriding current clip. </summary>
    /// <param name="clip"> Animation clip to be played. </param>
    public void PlayClip(Transform target, AnimationClip clip)
    {
        /* Enable The Animator */
        Animator.enabled = true;

        /* Warn User If NO State/Clip Exsists for This Animator */
        if (overrideController["Action"] == null) {
            Debug.LogError("No clip to override on state 'Action' for " + clip + 
                "Ensure that " + name + "s animator has a state called 'Action' with a clip called 'Action'");
            return;
        }

        /* Stop A Running Routine */
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        /* Set Target As Parent To The Camera */
        transform.SetParent(target);

        /* Override Empty "Action" Animation Clip and Trigger Animation */
        overrideController["Action"] = clip;
        Animator.Play("Action", 0, 0);

    }


    /// <summary>
    /// Plays a camera animation asset. Camera animation assets can be found in the asset menu.
    /// Animation will automatically accomadate for character size and orientation and follow the character.
    /// </summary>
    /// <param name="target"> The target of the animation. Its bounds are used to determine certain paramters. </param>
    /// <param name="camAnim"> Camera animation asset to play. </param>
    public void PlayAnimation(Collider target, CameraAnimation camAnim)
    {
        /* Disable Animator */
        Animator.enabled = false;

        /* De-Parent the Camera */
        transform.parent = null;

        /* Determine If The Camera Should Be Mirrored.
         * The camera should be mirrored if the angle between the arena's designated foward direction
         * and the targets right direction are more than 90 degrees apart. */
        float m = Vector3.Dot(BattleManager.instance.center.transform.right, target.transform.forward) < 0 ? -1 : 1;

        /* Offset The Camera's Position From Targets Center By Starting Offset */
        transform.position = new Vector3(
            camAnim.startOffset.x * m,
            camAnim.startOffset.y,
            camAnim.startOffset.z) +
            target.bounds.center;

        /* Locally Rotate The Camera By Starting Angle */
        transform.rotation = Quaternion.Euler(new Vector3(
            camAnim.startAngle.x,
            camAnim.startAngle.y * m,
            camAnim.startAngle.z));

        /* Start Animating The Camera */
        this.StartInterruptableCoroutine(camAnim.Animate(target, cam, m), ref currentRoutine);

    }


    /// <summary> Pans around specific target.</summary>
    /// <param name="target"> Collider target of animation. The camera will pan around this target. </param>
    /// <param name="distanceOffset"> Distance offset from target. 0 is no additional offset. (+) is further away. </param>
    /// <param name="verticalOffset"> Vertical offset from target. 0 is no additional offset. (+) is further up. </param>
    /// <param name="speed"> Speed of movement. Can be negative for reverse direction. </param>
    /// <param name="timeOffset"> Normalized Time in animation to start, from [0,1]. 0 is facing the back of target and 0.5 is facing the front. </param>
    public void PanAround(Collider target, float distanceOffset, float verticalOffset, float speed, float timeOffset)
    {
        /* Disable Animator And Nully Any Parents */
        Animator.enabled = false;

        /* Parent the Camera To The Target */
        transform.parent = null;

        /* If The Camera Should Be Mirrored...
         * The camera should be mirrored if the angle between the arena's designated foward direction
         * and the targets right direction are more than 90 degrees apart. */
        if(Vector3.Dot(BattleManager.instance.center.transform.forward, target.transform.right) < 0) 
        {
            /* Reverse The Speed And The Time Offset */
            speed *= -1;
            timeOffset = 1 - timeOffset;
        }

        /* Get Target Collider Size From It's Bounds */
        Vector3 objectSizes = target.bounds.max - target.bounds.min;

        /* Use The Maxium Bound Size As This Target's Size */
        float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);

        /* Calculate Current Field Of View For a Height Of 1 m */
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView);

        /* Calculate Wanted Distance From Target */
        float distance = 2.0f * objectSize / cameraView;

        /* Offset This Distance By The Target Size and Passed In Offset */
        distance += (0.5f + distanceOffset) * objectSize;

        /* Position The Camera Away From Target */
        transform.position = target.bounds.center + distance * target.transform.forward + Vector3.up * verticalOffset;
        
        /* Look At Center Of Target */
        transform.LookAt(target.bounds.center);

        /* Offset The Rotation Around The Target */
        transform.RotateAround(target.bounds.center, Vector3.up, timeOffset * 360);

        /* Start Paning The Camera */
        this.StartInterruptableCoroutine(Pan(target, speed), ref currentRoutine);
    }

    private IEnumerator Pan(Collider target, float speed)
    {
        /* Initialize Elapsed Time */
        float elapsedTime = 0;

        /* Continue While NOT Interrupted By Another Routine */
        while(speed != 0)
        {
            /* Wait One Frame */
            yield return null;

            /* Calculate Vertical Offset */
            Vector3 offset = 0.1f * Mathf.Sin(elapsedTime * speed) * Vector3.up;

            /* Offset The Camera */
            transform.position += 0.01f * offset;
            transform.RotateAround(target.bounds.center, Vector3.up, Time.deltaTime * 8 * speed);

            /* Increment Elapsed Time and Wait Until Next Frame */
            elapsedTime += Time.deltaTime;
            

        }
    }
}





