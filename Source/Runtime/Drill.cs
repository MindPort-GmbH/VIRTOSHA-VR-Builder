using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    [RequireComponent(typeof(IUsableProperty))]
    public class Drill : MonoBehaviour
    {
        DrillBit drillBit;
        IUsableProperty usableProperty;
        IDrillableProperty currentDrilledObject = null;
        Vector3 drillStartPosition;
        Vector3 drillDirection;
        float drillDistance = 0f;
        float maxDeviation = 0.05f;
        bool isDrilling;

        private void OnEnable()
        {
            if (drillBit == null)
            {
                drillBit = GetComponentInChildren<DrillBit>();
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
            drillBit.TouchedDrillableObject += OnTouchedDrillableObject;
            drillBit.IsUsing = true;
        }

        private void OnTouchedDrillableObject(object sender, DrillBitEventArgs e)
        {
            if (currentDrilledObject != null)
            {
                return;
            }

            currentDrilledObject = e.DrillableProperty;
            drillStartPosition = GetClosestPointOnCollider(drillBit.Base.position, e.OtherCollider);
            drillDirection = (drillBit.transform.rotation * Vector3.forward).normalized;
            isDrilling = true;
        }

        private Vector3 GetClosestPointOnCollider(Vector3 position, Collider otherCollider)
        {
            Vector3 closestPoint = otherCollider.ClosestPoint(position);

            return closestPoint;
        }

        private void OnUseEnded(UsablePropertyEventArgs args)
        {
            drillBit.IsUsing = false;

            if (isDrilling == false)
            {
                return;
            }

            drillBit.TouchedDrillableObject -= OnTouchedDrillableObject;

            StopDrilling();
        }

        private void StopDrilling()
        {
            isDrilling = false;

            if (currentDrilledObject != null)
            {
                Vector3 drillEndPosition = drillStartPosition + drillDirection * drillDistance;

                currentDrilledObject.CreateHole(drillStartPosition, drillEndPosition, drillBit.Width);

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

            if (CalculateDeviation(drillStartPosition, drillDirection, drillBit.Tip.position) > maxDeviation)
            {
                StopDrilling();
                return;
            }

            drillDistance = Mathf.Max(drillDistance, Vector3.Distance(drillStartPosition, drillBit.Tip.position));
        }

        private void OnDrawGizmos()
        {
            if (drillBit == null || isDrilling == false)
            {
                return;
            }

            Vector3 drillEndPosition = drillStartPosition + drillDirection * drillDistance;
            DebugUtils.DrawCylinderGizmo(drillStartPosition, drillEndPosition, drillBit.Width, UnityEngine.Color.red);
        }

        public float CalculateDeviation(Vector3 origin, Vector3 direction, Vector3 currentPosition)
        {
            direction.Normalize();
            Vector3 originToCurrent = currentPosition - origin;
            float projectionLength = Vector3.Dot(originToCurrent, direction);
            Vector3 closestPointOnRay = origin + projectionLength * direction;
            float distance;

            if (projectionLength < 0)
            {
                // If the closest point is the origin, it means we're behind the drilled hole - stop drilling immediately.
                distance = Vector3.Distance(currentPosition, origin);
            }
            else
            {
                distance = Vector3.Distance(currentPosition, closestPointOnRay);
            }

            return distance;
        }
    }
}