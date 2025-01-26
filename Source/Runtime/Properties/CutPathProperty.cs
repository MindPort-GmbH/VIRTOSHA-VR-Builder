using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace VRBuilder.VIRTOSHA.Properties
{
    public class CutPathProperty : FollowPathProperty
    {
                protected override void EvaluateDeviations()
        {
            // 1) Basic checks
            if (!BasicChecks())
            {
                return;
            }

            // 2) Retrieve the first spline
            Spline spline = splineContainer.Splines[0];

            // 3) Get tip's world position
            Transform tipTransform = CurrentFollowPathObjectProperty.FollowPathTip.TipTransform;
            Vector3 tipPosWorld = tipTransform.position;
            Debug.Log($"[CutPathProperty] Tip Position (world): {tipPosWorld}");

            // -- TRANSFORM TIP INTO SPLINE’S LOCAL SPACE --
            Vector3 tipPosLocal = splineContainer.transform.InverseTransformPoint(tipPosWorld);

            // 4) Find the nearest point on the spline (in local coords)
            float3 nearestLocal;
            float tVal;
            SplineUtility.GetNearestPoint(spline, tipPosLocal, out nearestLocal, out tVal, resolution: 8, iterations: 4);
            Debug.Log($"[CutPathProperty] Nearest local: {nearestLocal}, tVal: {tVal}");

            // 5) Convert that nearest point back to world space
            Vector3 nearestWorld = splineContainer.transform.TransformPoint((Vector3)nearestLocal);
            Debug.Log($"[CutPathProperty] Nearest point in world space: {nearestWorld}");

            // 6) Compute the offset in world space
            Vector3 offset = tipPosWorld - nearestWorld;
            Debug.Log($"[CutPathProperty] Offset (world): {offset}, magnitude: {offset.magnitude}");

            // 7) Build a local frame for measuring up/down, etc.
            float3 tangentLocal = math.normalize(SplineUtility.EvaluateTangent(spline, tVal));
            // Convert the spline tangent to world space as well
            Vector3 tangentWorld = splineContainer.transform.TransformVector((Vector3)tangentLocal).normalized;

            // For example, pick a cross reference (world up),
            // then define binormal/normal in world space:
            Vector3 crossRef = Vector3.up;
            Vector3 binormal = Vector3.Cross(tangentWorld, crossRef).normalized;
            Vector3 normal = Vector3.Cross(binormal, tangentWorld).normalized;

            // 8) Tip's Y+ (tipTransform.up) is physically "down," and offsetUpDown > 0 means "inside,"
            float offsetUpDown = Vector3.Dot(offset, tipTransform.up);
            Debug.Log($"[CutPathProperty] offsetUpDown = {offsetUpDown}");

            if (offsetUpDown > 0f)
            {
                Debug.Log("[CutPathProperty] Tip is inside the skin - evaluating standard deviations");
                // 10) Call the shared logic
                EvaluateStandardDeviations(tVal, offset, normal, binormal, tangentWorld);
            }
        }
    }
}
