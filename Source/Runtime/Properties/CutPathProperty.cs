using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

namespace VRBuilder.VIRTOSHA.Properties
{

    public class CutPathProperty : FollowPathProperty
    {
        protected override void EvaluateDeviations()
        {
            if (CurrentFollowPathObjectProperty == null ||
                CurrentFollowPathObjectProperty.FollowPathTip == null ||
                splineContainer == null ||
                splineContainer.Splines.Count == 0)
            {
                return;
            }

            Spline spline = splineContainer.Splines[0];
            Vector3 tipPos = CurrentFollowPathObjectProperty.FollowPathTip.TipPosition;

            float3 nearest;
            float tVal;
            SplineUtility.GetNearestPoint(spline, tipPos, out nearest, out tVal);

            Vector3 offset = tipPos - (Vector3)nearest;

            // Build local frame
            float3 tangentF3 = SplineUtility.EvaluateTangent(spline, tVal);
            Vector3 tangent = ((Vector3)tangentF3).normalized;
            Vector3 crossRef = Vector3.up;
            Vector3 binormal = Vector3.Cross(tangent, crossRef).normalized;
            Vector3 normal = Vector3.Cross(binormal, tangent).normalized;

            float offsetUpDown = Vector3.Dot(offset, normal);

            // "Cutting" constraint: only do deviation checks if tip is below the spline
            if (offsetUpDown < 0f)
            {
                // Now we call the shared logic
                EvaluateStandardDeviations(tVal, offset, normal, binormal, tangent);
            }
            // Otherwise, do nothing (no progress, no failure).
        }
    }
}
