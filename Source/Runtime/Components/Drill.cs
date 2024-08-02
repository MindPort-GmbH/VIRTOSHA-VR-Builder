using System;
using System.Linq;
using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.Core.Utils;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Component defining a drilling tool that can be activated by holding the use action.
    /// Requires a <see cref="DrillBit"/> component on a child object.
    /// </summary>
    [RequireComponent(typeof(IUsableProperty))]
    public class Drill : MonoBehaviour, IDrill
    {
        DrillBit drillBit;
        IUsableProperty usableProperty;
        IDrillableProperty currentDrilledObject = null;
        Vector3 drillStartPosition;
        Vector3 drillDirection;
        float currentDrillDepth = 0f;
        bool isDrilling;
        private DrillingAffordance affordanceObject;

        [SerializeField]
        [Tooltip("Distance from the drilled axis at which the drilling is automatically interrupted.")]
        float maxDeviation = 0.05f;

        [SerializeField]
        [Tooltip("If present, this prefab will be spawned at the location of a hole being drilled to visualize it.")]
        private DrillingAffordance affordancePrefab;

        /// <inheritdoc/>
        public bool IsDrilling => isDrilling;

        /// <inheritdoc/>
        public float CurrentDrillDepth => currentDrillDepth;

        /// <inheritdoc/>
        public event EventHandler<DrillEventArgs> DrillingStarted;

        /// <inheritdoc/>
        public event EventHandler<DrillEventArgs> DrillingStopped;

        private void OnEnable()
        {
            if (drillBit == null)
            {
                drillBit = GetComponentInChildren<DrillBit>();
            }

            if (drillBit == null)
            {
                Debug.LogError($"The {typeof(Drill).Name} component on '{gameObject.name}' cannot work without a {typeof(DrillBit).Name} component on a child object. Please create one.");
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
            drillBit.InUse = true;
        }

        private void OnTouchedDrillableObject(object sender, DrillBitEventArgs e)
        {
            if (currentDrilledObject != null)
            {
                return;
            }

            currentDrilledObject = e.DrillableProperty;
            drillStartPosition = GetClosestPointOnCollider(e.OtherCollider);
            drillDirection = (drillBit.transform.rotation * Vector3.forward).normalized;
            isDrilling = true;
            VisualizeDrillingAffordance();
            DrillingStarted?.Invoke(this, new DrillEventArgs(drillStartPosition, drillDirection, drillBit.Width));
        }

        private void VisualizeDrillingAffordance()
        {
            if (affordancePrefab == null)
            {
                return;
            }

            affordanceObject = GameObject.Instantiate<DrillingAffordance>(affordancePrefab);
            affordanceObject.transform.position = drillStartPosition;
            affordanceObject.transform.rotation = Quaternion.LookRotation(drillDirection);
            affordanceObject.SetWidth(drillBit.Width);
        }

        private void RemoveDrillingAffordance()
        {
            affordanceObject?.Remove();
        }

        private Vector3 GetClosestPointOnCollider(Collider otherCollider)
        {
            Ray ray = new Ray(drillBit.Base.position, drillBit.Tip.position - drillBit.Base.position);
            RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(drillBit.Base.position, drillBit.Tip.position));

            if (hits.Any(hit => hit.collider == otherCollider))
            {
                RaycastHit hit = hits.First(hit => hit.collider == otherCollider);
                return hit.point;
            }

            return drillBit.Tip.position;
        }

        private void OnUseEnded(UsablePropertyEventArgs args)
        {
            drillBit.InUse = false;

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
                Vector3 drillEndPosition = drillStartPosition + drillDirection * currentDrillDepth;

                currentDrilledObject.CreateHole(drillStartPosition, drillEndPosition, drillBit.Width);

                currentDrilledObject = null;
                currentDrillDepth = 0f;
                RemoveDrillingAffordance();
                DrillingStopped?.Invoke(this, new DrillEventArgs(drillStartPosition, drillDirection, drillBit.Width));
            }
        }

        private void Update()
        {
            if (isDrilling == false)
            {
                return;
            }

            float projectionLength;
            if (CalculateDeviation(drillStartPosition, drillDirection, drillBit.Tip.position, out projectionLength) > maxDeviation)
            {
                StopDrilling();
                return;
            }

            if (projectionLength > 0)
            {
                currentDrillDepth = Mathf.Max(currentDrillDepth, Vector3.Distance(drillStartPosition, drillBit.Tip.position));
            }

            affordanceObject?.SetDepth(CurrentDrillDepth);
        }

        private void OnDrawGizmos()
        {
            if (drillBit == null || isDrilling == false)
            {
                return;
            }

            Vector3 drillEndPosition = drillStartPosition + drillDirection * currentDrillDepth;
            DebugUtils.DrawWireCylinderGizmo(drillStartPosition, drillEndPosition, drillBit.Width, Color.red);
        }

        private float CalculateDeviation(Vector3 origin, Vector3 direction, Vector3 currentPosition, out float projectionLength)
        {
            direction.Normalize();
            Vector3 originToCurrent = currentPosition - origin;
            projectionLength = Vector3.Dot(originToCurrent, direction);
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