using System.Linq;
using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    [RequireComponent(typeof(IUsableProperty))]
    public class Drill : MonoBehaviour
    {
        DrillTip drillTip;
        IUsableProperty usableProperty;
        IDrillableProperty currentDrilledObject = null;
        Vector3 drillStartPosition;
        Vector3 drillDirection;
        float drillDistance = 0f;
        float holeWidth = 0.05f;
        float maxDeviation = 0.05f;
        bool isDrilling;

        private void OnEnable()
        {
            if (drillTip == null)
            {
                drillTip = GetComponentInChildren<DrillTip>();
            }

            if (usableProperty == null)
            {
                usableProperty = GetComponent<IUsableProperty>();
            }

            usableProperty.UseStarted.AddListener(OnUseStarted);
            usableProperty.UseEnded.AddListener(OnUseEnded);

            //DEBUG
            OnUseStarted(new UsablePropertyEventArgs());
        }

        private void OnDisable()
        {
            usableProperty?.UseStarted.RemoveListener(OnUseStarted);
            usableProperty?.UseEnded.RemoveListener(OnUseEnded);
        }

        private void OnUseStarted(UsablePropertyEventArgs arg0)
        {
            drillTip.TouchedDrillableObject += OnTouchedDrillableObject;
            drillTip.IsUsing = true;
        }

        private void OnTouchedDrillableObject(object sender, DrillTipEventArgs e)
        {
            if (currentDrilledObject != null)
            {
                return;
            }

            currentDrilledObject = e.DrillableProperty;
            drillStartPosition = GetClosestPointOnCollider(drillTip.transform.position, e.DrillableProperty.SceneObject.GameObject);
            drillDirection = (drillTip.transform.rotation * Vector3.forward).normalized;
            isDrilling = true;
        }

        private Vector3 GetClosestPointOnCollider(Vector3 position, GameObject gameObject)
        {
            Vector3[] closestPoints = gameObject.GetComponentsInChildren<Collider>().Select(collider => collider.ClosestPoint(position)).ToArray();

            Vector3 closestPoint = closestPoints.First();
            float currentDistance = Vector3.Distance(position, closestPoint);

            for (int i = 0; i < closestPoints.Length; i++)
            {
                float distance = Vector3.Distance(position, closestPoints[i]);
                if (distance < currentDistance)
                {
                    closestPoint = closestPoints[i];
                    currentDistance = distance;
                }
            }

            Debug.DrawLine(position, closestPoint, UnityEngine.Color.red, 5.0f);

            return closestPoint;
        }

        private void OnUseEnded(UsablePropertyEventArgs args)
        {
            drillTip.IsUsing = false;

            if (isDrilling == false)
            {
                return;
            }

            drillTip.TouchedDrillableObject -= OnTouchedDrillableObject;
            StopDrilling();
        }

        private void StopDrilling()
        {
            isDrilling = false;

            if (currentDrilledObject != null)
            {
                Vector3 drillEndPosition = drillStartPosition + drillDirection * drillDistance;

                currentDrilledObject.CreateHole(drillStartPosition, drillEndPosition, holeWidth);

                currentDrilledObject = null;
                drillDistance = 0f;
            }
        }

        private void Update()
        {
            if (isDrilling == false)
            {
                return;
            }

            if (CalculateDeviation(drillStartPosition, drillDirection, drillTip.transform.position) > maxDeviation)
            {
                StopDrilling();
            }

            drillDistance = Mathf.Max(drillDistance, Vector3.Distance(drillStartPosition, drillTip.transform.position));
        }

        private void OnDrawGizmos()
        {
            if (drillTip == null || isDrilling == false)
            {
                return;
            }

            Vector3 drillEndPosition = drillStartPosition + drillDirection * drillDistance;
            DebugUtils.DrawCylinderGizmo(drillStartPosition, drillEndPosition, holeWidth, UnityEngine.Color.red);
        }

        public float CalculateDeviation(Vector3 origin, Vector3 direction, Vector3 currentPosition)
        {
            direction.Normalize();
            Vector3 originToCurrent = currentPosition - origin;
            float projectionLength = Vector3.Dot(originToCurrent, direction);
            Vector3 closestPointOnRay;

            if (projectionLength < 0)
            {
                // If the closest point is the origin, it means we're behind the drilled hole - stop drilling immediately.
                return float.MaxValue;
            }
            else
            {
                closestPointOnRay = origin + projectionLength * direction;
            }

            float distance = Vector3.Distance(currentPosition, closestPointOnRay);

            return distance;
        }
    }
}