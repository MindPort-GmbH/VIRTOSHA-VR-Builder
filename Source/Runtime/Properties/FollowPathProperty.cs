using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using VRBuilder.Core.Properties;
using Unity.Mathematics;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Stores data about a spline path, including max deviations, angle constraints, fail conditions, and UnityEvents.
    /// Updated to use SplineUtility.GetNearestPoint instead of SplineUtility.Project.
    /// </summary>
    [RequireComponent(typeof(SplineContainer))]
    public class FollowPathProperty : LockableProperty, IFollowPathProperty
    {
        [Header("Spline Reference")]
        [SerializeField]
        protected SplineContainer splineContainer;

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
        [Tooltip("Target roll angle (degrees) around the path's forward axis.")]
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
        [Range(0f, 1f)]
        public float pathCompleted = 0f;

        [Tooltip("If enabled, pathCompleted will be reset to 0 if a max deviation is exceeded.")]
        public bool resetPathCompletedOnDeviation = false;

        [Header("Events")]
        public UnityEvent OnPathFail;
        public UnityEvent OnPathCompleted;

        [Header("Visualization")]
        public SplineInstantiate pathVisualization;
        public SplineInstantiate pathProgressVisualization;

        public bool IsPathCompleted { get; private set; }

        /// <summary>
        /// Reference to the object/tip that is currently following this path.
        /// </summary>
        public IFollowPathObjectProperty CurrentFollowPathObjectProperty { get; set; }

        private float totalLength;

        private void Awake()
        {
            if (splineContainer == null)
            {
                splineContainer = GetComponent<SplineContainer>();
            }

            // Example: compute total length of the first Spline in the container
            if (splineContainer.Splines.Count > 0)
            {
                var spline = splineContainer.Splines[0];
                totalLength = spline.GetLength(); // in your snippet: SplineUtility.GetLength(spline)
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

        protected virtual void Update()
        {
            EvaluateDeviations();

            if (pathCompleted >= 1f && IsPathCompleted == false)
            {
                IsPathCompleted = true;
                OnPathCompleted?.Invoke();
            }
        }

        /// <summary>
        /// The default EvaluateDeviations calls the helper once we have offset & tVal.
        /// </summary>
        protected virtual void EvaluateDeviations()
        {
            // Basic checks
            if (CurrentFollowPathObjectProperty == null ||
                CurrentFollowPathObjectProperty.FollowPathTip == null ||
                splineContainer == null ||
                splineContainer.Splines.Count == 0)
            {
                return;
            }

            var spline = splineContainer.Splines[0];
            Vector3 tipPos = CurrentFollowPathObjectProperty.FollowPathTip.TipPosition;

            float3 nearest;
            float tVal;
            SplineUtility.GetNearestPoint(spline, tipPos, out nearest, out tVal);

            Vector3 offset = tipPos - (Vector3)nearest;

            // Build local tangent frame
            float3 tangentF3 = SplineUtility.EvaluateTangent(spline, tVal);
            Vector3 tangent = ((Vector3)tangentF3).normalized;
            Vector3 crossRef = Vector3.up;
            Vector3 binormal = Vector3.Cross(tangent, crossRef).normalized;
            Vector3 normal = Vector3.Cross(binormal, tangent).normalized;

            // Then call the helper
            EvaluateStandardDeviations(tVal, offset, normal, binormal, tangent);
        }

        /// <summary>
        /// Contains the 'shared' logic for computing pathCompleted, checking left/right/up/down, angles, and triggering failure.
        /// Subclasses can call this whenever they want.
        /// </summary>
        protected void EvaluateStandardDeviations(float tVal, Vector3 offset, Vector3 normal, Vector3 binormal, Vector3 tangent)
        {
            // 1) Update pathCompleted
            float distanceAlongSpline = tVal * totalLength;
            pathCompleted = Mathf.Clamp01(distanceAlongSpline / totalLength);

            // 2) Deviation checks
            float offsetUpDown = Vector3.Dot(offset, normal);
            float offsetLeftRight = Vector3.Dot(offset, binormal);
            bool fail = false;

            // up/down
            if (offsetUpDown > maxDeviationUp && failConditionUp)
                fail = true;
            else if (offsetUpDown < -maxDeviationDown && failConditionDown)
                fail = true;

            // left/right
            if (offsetLeftRight > maxDeviationLeft && failConditionLeft)
                fail = true;
            else if (offsetLeftRight < -maxDeviationRight && failConditionRight)
                fail = true;

            // angles
            if (!ignorePitchAndRoll)
            {
                Vector3 tipForward = CurrentFollowPathObjectProperty.FollowPathTip.TipRotation.normalized;
                float currentRoll = SignedRollAngle(tipForward, tangent, normal);
                if (Mathf.Abs(currentRoll - targetAngleRoll) > maxAngleDeviationRoll && failConditionRoll)
                    fail = true;

                float currentPitch = SignedPitchAngle(tipForward, tangent, normal);
                if (Mathf.Abs(currentPitch - targetAnglePitch) > maxAngleDeviationPitch && failConditionPitch)
                    fail = true;
            }

            // 3) If any fail, handle reset and event
            if (fail)
            {
                if (resetPathCompletedOnDeviation)
                {
                    pathCompleted = 0f;
                    IsPathCompleted = false;
                }
                OnPathFail?.Invoke();
            }
        }

        /// <summary>
        /// Example method measuring roll about 'tangent' axis.
        /// </summary>
        private float SignedRollAngle(Vector3 tipForward, Vector3 pathForward, Vector3 pathUp)
        {
            // Compute rotation from pathForward to tipForward
            Quaternion fromTo = Quaternion.FromToRotation(pathForward, tipForward);

            // "Roll" is how much pathUp rotates about pathForward
            Vector3 newUp = fromTo * pathUp;
            float angle = Vector3.SignedAngle(pathUp, newUp, pathForward);
            return angle;
        }

        /// <summary>
        /// Example method measuring pitch in plane of pathForward & pathUp.
        /// </summary>
        private float SignedPitchAngle(Vector3 tipForward, Vector3 pathForward, Vector3 pathUp)
        {
            // Project tipForward into plane spanned by pathForward & pathUp
            Vector3 sideAxis = Vector3.Cross(pathForward, pathUp);
            Vector3 forwardProj = Vector3.ProjectOnPlane(tipForward, sideAxis);

            float angle = Vector3.SignedAngle(pathForward, forwardProj, sideAxis);
            return angle;
        }

        public void CompletePath()
        {
            pathCompleted = 1f;
            IsPathCompleted = true;
        }

        public void AddDefaultTrajectoryValues()
        {
            maxDeviationUp = 0.02f;
            maxDeviationDown = 0.02f;
            maxDeviationLeft = 0.01f;
            maxDeviationRight = 0.01f;
            targetAngleRoll = 0f;
            maxAngleDeviationRoll = 5f;
            targetAnglePitch = 0f;
            maxAngleDeviationPitch = 5f;
        }

        public void AddDefaultVisualization()
        {
            if (pathVisualization == null)
            {
                pathVisualization = gameObject.AddComponent<SplineInstantiate>();
            }
        }

        public void AddDefaultProgressVisualization()
        {
            if (pathProgressVisualization == null)
            {
                pathProgressVisualization = gameObject.AddComponent<SplineInstantiate>();
            }
        }

        protected override void InternalSetLocked(bool lockState)
        {
            // No locking implementation needed
        }
    }
}
