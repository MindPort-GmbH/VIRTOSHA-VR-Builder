/*
<ai_context>
  Implementation for IFollowPathProperty which tracks a spline path and measures deviation and angles.
</ai_context>
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines; // from com.unity.splines
using VRBuilder.Core.Properties;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Stores data about a spline path, including max deviations, angle constraints, fail conditions, and UnityEvents.
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    public class FollowPathProperty : LockableProperty, IFollowPathProperty
    {
        [Header("Spline Reference")]
        [SerializeField]
        private SplineContainer splineContainer;

        [Header("Deviation Settings")]
        [Tooltip("Allowed deviation above the spline (Y+). Negative means must stay below this level (minimum depth).")]
        public float maxDeviationUp = 0.0f;

        [Tooltip("Allowed deviation below the spline (Y-).")]
        public float maxDeviationDown = 0.0f;

        [Tooltip("Allowed deviation to the left (Z+).")]
        public float maxDeviationLeft = 0.0f;

        [Tooltip("Allowed deviation to the right (Z-).")]
        public float maxDeviationRight = 0.0f;

        [Header("Angles")]
        [Tooltip("Target roll angle (degrees) around the forward axis.")]
        public float targetAngleRoll = 0.0f;

        [Tooltip("Maximum deviation allowed from the target roll.")]
        public float maxAngleDeviationRoll = 5.0f;

        [Tooltip("Target pitch angle (degrees) around the X axis.")]
        public float targetAnglePitch = 0.0f;

        [Tooltip("Maximum deviation allowed from the target pitch.")]
        public float maxAngleDeviationPitch = 5.0f;

        [Header("Fail Conditions")]
        [Tooltip("Trigger a fail event if we exceed max deviation up.")]
        public bool failConditionUp = false;

        [Tooltip("Trigger a fail event if we exceed max deviation down.")]
        public bool failConditionDown = false;

        [Tooltip("Trigger a fail event if we exceed max deviation left.")]
        public bool failConditionLeft = false;

        [Tooltip("Trigger a fail event if we exceed max deviation right.")]
        public bool failConditionRight = false;

        [Tooltip("Trigger a fail event if we exceed roll angle deviation.")]
        public bool failConditionRoll = false;

        [Tooltip("Trigger a fail event if we exceed pitch angle deviation.")]
        public bool failConditionPitch = false;

        [Header("Angle Options")]
        [Tooltip("Ignore pitch & roll angles entirely if set to true.")]
        public bool ignorePitchAndRoll = false;

        [Header("Progress")]
        [Tooltip("Tracks progress along the path (0 to 1 if desired).")]
        [Range(0f, 1f)]
        public float pathCompleted = 0f;

        [Tooltip("If enabled, pathCompleted will be reset to 0 if a max deviation is exceeded.")]
        public bool resetPathCompletedOnDeviation = false;

        [Header("Events")]
        [Tooltip("Called when any max deviation occurs that has failCondition enabled.")]
        public UnityEvent OnPathFail;

        [Tooltip("Called when the path was successfully completed.")]
        public UnityEvent OnPathCompleted;

        [Header("Visualization")]
        [Tooltip("SplineInstantiate for the path to be followed.")]
        public SplineInstantiate pathVisualization;

        [Tooltip("SplineInstantiate for the portion of the path completed (progress).")]
        public SplineInstantiate pathProgressVisualization;

        /// <summary>
        /// True if the path is fully completed. 
        /// You could also interpret 'pathCompleted >= 1f' as done.
        /// </summary>
        public bool IsPathCompleted { get; private set; }

        public IEnumerable<Collider> Colliders => throw new System.NotImplementedException();


        private void Awake()
        {
            if (splineContainer == null)
            {
                splineContainer = GetComponent<SplineContainer>();
            }
        }

        private void OnValidate()
        {
            // Ensure the absolute value of a negative maxDeviationUp is never bigger than maxDeviationDown.
            if (maxDeviationUp < 0f)
            {
                float absUp = Mathf.Abs(maxDeviationUp);
                if (absUp > maxDeviationDown)
                {
                    maxDeviationUp = -maxDeviationDown;
                }
            }
        }

        /// <summary>
        /// Force-complete the path (e.g. for fast-forwarding in VR Builder).
        /// </summary>
        public void CompletePath()
        {
            pathCompleted = 1f;
            IsPathCompleted = true;
        }

        /// <summary>
        /// Example UI button to add default trajectory values.
        /// </summary>
        public void AddDefaultTrajectoryValues()
        {
            // Example preset for demonstration
            maxDeviationUp = 0.02f;
            maxDeviationDown = 0.02f;
            maxDeviationLeft = 0.01f;
            maxDeviationRight = 0.01f;
            targetAngleRoll = 0f;
            maxAngleDeviationRoll = 5f;
            targetAnglePitch = 0f;
            maxAngleDeviationPitch = 5f;
        }

        /// <summary>
        /// Example UI button to add default visualization for the path to be followed.
        /// </summary>
        public void AddDefaultVisualisation()
        {
            if (pathVisualization == null)
            {
                pathVisualization = gameObject.AddComponent<SplineInstantiate>();
                // You could configure it here as needed...
            }
        }

        /// <summary>
        /// Example UI button to add default visualization for path progress.
        /// </summary>
        public void AddDefaultProgressVisualisation()
        {
            if (pathProgressVisualization == null)
            {
                pathProgressVisualization = gameObject.AddComponent<SplineInstantiate>();
                // You could configure a different style here...
            }
        }

        protected override void InternalSetLocked(bool lockState)
        {
            // Example lock logic if needed.
        }
    }
}