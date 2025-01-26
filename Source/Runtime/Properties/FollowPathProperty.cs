using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using VRBuilder.Core.Properties;
using VRBuilder.VIRTOSHA.Components;

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
        public IFollowPathObjectTip FollowPathTip { get; private set; }

        /// <summary>
        /// For IFollowPathObjectProperty, if needed to store.
        /// </summary>
        public IFollowPathObjectProperty CurrentFollowPathObjectProperty { get; set; }

        private float totalLength;

        private void Awake()
        {
            if (splineContainer == null)
            {
                splineContainer = GetComponent<SplineContainer>();
            }

            if (splineContainer.Splines.Count > 0)
            {
                var spline = splineContainer.Splines[0];
                totalLength = spline.GetLength();
                LogSplineDetails(spline);
            }
        }

        private void LogSplineDetails(Spline spline)
        {
            Debug.Log($"[FollowPathProperty] Spline Details:");
            Debug.Log($"  - Total Length: {totalLength:F3}");
            Debug.Log($"  - Knot Count: {spline.Count}");
            float3 startPoint = spline.EvaluatePosition(0f);
            float3 endPoint = spline.EvaluatePosition(1f);
            Debug.Log($"  - Start Point (local): {startPoint}");
            Debug.Log($"  - End Point (local): {endPoint}");
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
                Debug.Log("[FollowPathProperty] Path completed, triggering completion event");
                IsPathCompleted = true;
                OnPathCompleted?.Invoke();
            }
        }

        protected virtual void EvaluateDeviations()
        {
            Debug.Log("[FollowPathProperty] Starting deviation evaluation");

            if (!BasicChecks())
            {
                return;
            }

            var spline = splineContainer.Splines[0];
            Vector3 tipPos = CurrentFollowPathObjectProperty.FollowPathTip.TipTransform.position;
            Vector3 tipPosLocal = splineContainer.transform.InverseTransformPoint(tipPos);
            Debug.Log($"[FollowPathProperty] Tip Position / Local: {tipPos} {tipPosLocal}");


            // 1) Get local nearest point
            float3 nearestLocal;
            float tVal;
            SplineUtility.GetNearestPoint(spline, tipPos, out nearestLocal, out tVal);
            Debug.Log($"[FollowPathProperty] Nearest point on spline (local): {nearestLocal}, t-value: {tVal}");

            // 2) Transform local to world
            Matrix4x4 localToWorld = splineContainer.transform.localToWorldMatrix;
            Vector3 nearestWorld = localToWorld.MultiplyPoint3x4((Vector3)nearestLocal);
            Debug.Log($"[FollowPathProperty] Nearest point in world space: {nearestWorld}");

            // 3) Compute offset in world space
            Vector3 offset = tipPos - nearestWorld;
            Debug.Log($"[FollowPathProperty] Offset from spline: {offset}, magnitude: {offset.magnitude}");

            // 4) Build local tangent frame in local space, then also transform to world.
            float3 tangentLocal = math.normalize(SplineUtility.EvaluateTangent(spline, tVal));
            // Transform direction with rotation (no position offset).
            Vector3 tangentWorld = localToWorld.MultiplyVector((Vector3)tangentLocal).normalized;

            Vector3 crossRef = Vector3.up;
            Vector3 binormal = Vector3.Cross(tangentWorld, crossRef).normalized;
            Vector3 normal = Vector3.Cross(binormal, tangentWorld).normalized;
            Debug.Log($"[FollowPathProperty] Local frame (world) - Tangent: {tangentWorld}, Normal: {normal}, Binormal: {binormal}");

            EvaluateStandardDeviations(tVal, offset, normal, binormal, tangentWorld);
        }

        protected bool BasicChecks()
        {
            bool result = CurrentFollowPathObjectProperty != null &&
                          CurrentFollowPathObjectProperty.FollowPathTip != null &&
                          splineContainer != null &&
                          splineContainer.Splines.Count > 0;
            return result;
        }

        protected void EvaluateStandardDeviations(float tVal, Vector3 offset, Vector3 normal, Vector3 binormal, Vector3 tangent)
        {
            Debug.Log("[FollowPathProperty] Starting standard deviation evaluation");

            float distanceAlongSpline = tVal * totalLength;
            pathCompleted = Mathf.Clamp01(distanceAlongSpline / totalLength);
            Debug.Log($"[FollowPathProperty] Path completion: {pathCompleted:F2} (distance: {distanceAlongSpline:F2})");

            float offsetUpDown = Vector3.Dot(offset, normal);
            float offsetLeftRight = Vector3.Dot(offset, binormal);
            Debug.Log($"[FollowPathProperty] Deviations - Up/Down: {offsetUpDown:F3}, Left/Right: {offsetLeftRight:F3}");

            bool fail = false;

            // Up/down checks
            if (offsetUpDown > maxDeviationUp && failConditionUp)
            {
                Debug.Log($"[FollowPathProperty] Failed: Exceeded max up deviation ({offsetUpDown:F3} > {maxDeviationUp})");
                fail = true;
            }
            else if (offsetUpDown < -maxDeviationDown && failConditionDown)
            {
                Debug.Log($"[FollowPathProperty] Failed: Exceeded max down deviation ({offsetUpDown:F3} < -{maxDeviationDown})");
                fail = true;
            }

            // Left/right checks
            if (offsetLeftRight > maxDeviationLeft && failConditionLeft)
            {
                Debug.Log($"[FollowPathProperty] Failed: Exceeded max left deviation ({offsetLeftRight:F3} > {maxDeviationLeft})");
                fail = true;
            }
            else if (offsetLeftRight < -maxDeviationRight && failConditionRight)
            {
                Debug.Log($"[FollowPathProperty] Failed: Exceeded max right deviation ({offsetLeftRight:F3} < -{maxDeviationRight})");
                fail = true;
            }

            // angles
            if (!ignorePitchAndRoll)
            {
                Vector3 tipForward = CurrentFollowPathObjectProperty.FollowPathTip.TipTransform.forward;
                float currentRoll = SignedRollAngle(tipForward, tangent, normal);
                float currentPitch = SignedPitchAngle(tipForward, tangent, normal);
                Debug.Log($"[FollowPathProperty] Current angles - Roll: {currentRoll:F1}°, Pitch: {currentPitch:F1}°");

                if (Mathf.Abs(currentRoll - targetAngleRoll) > maxAngleDeviationRoll && failConditionRoll)
                {
                    Debug.Log($"[FollowPathProperty] Failed: Exceeded max roll deviation (current: {currentRoll:F1}°, target: {targetAngleRoll}° ±{maxAngleDeviationRoll}°)");
                    fail = true;
                }

                if (Mathf.Abs(currentPitch - targetAnglePitch) > maxAngleDeviationPitch && failConditionPitch)
                {
                    Debug.Log($"[FollowPathProperty] Failed: Exceeded max pitch deviation (current: {currentPitch:F1}°, target: {targetAnglePitch}° ±{maxAngleDeviationPitch}°)");
                    fail = true;
                }
            }

            if (fail)
            {
                Debug.Log("[FollowPathProperty] Failure detected, handling reset and event");
                if (resetPathCompletedOnDeviation)
                {
                    pathCompleted = 0f;
                    IsPathCompleted = false;
                }
                OnPathFail?.Invoke();
            }
        }

        private float SignedRollAngle(Vector3 tipForward, Vector3 pathForward, Vector3 pathUp)
        {
            Quaternion fromTo = Quaternion.FromToRotation(pathForward, tipForward);
            Vector3 newUp = fromTo * pathUp;
            float angle = Vector3.SignedAngle(pathUp, newUp, pathForward);
            return angle;
        }

        private float SignedPitchAngle(Vector3 tipForward, Vector3 pathForward, Vector3 pathUp)
        {
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