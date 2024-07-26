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
        IDrillableProperty currentDrilledObject;
        Vector3 drillStartPosition;
        Vector3 drillDirection;
        float drillDistance = 0f;
        float holeWidth = 0.05f;

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
        }

        private void OnDisable()
        {
            usableProperty?.UseStarted.RemoveListener(OnUseStarted);
            usableProperty?.UseEnded.RemoveListener(OnUseEnded);
        }

        private void OnUseStarted(UsablePropertyEventArgs arg0)
        {
            drillTip.IsDrilling = true;
            drillTip.TouchedDrillableObject += OnTouchedDrillableObject;
        }

        private void OnTouchedDrillableObject(object sender, DrillTipEventArgs e)
        {
            if (currentDrilledObject != null)
            {
                return;
            }

            currentDrilledObject = e.DrillableProperty;
            drillStartPosition = drillTip.transform.position;
            drillDirection = (drillTip.transform.rotation * Vector3.forward).normalized;
        }

        private void OnUseEnded(UsablePropertyEventArgs args)
        {
            if (drillTip.IsDrilling == false)
            {
                return;
            }

            drillTip.IsDrilling = false;
            drillTip.TouchedDrillableObject -= OnTouchedDrillableObject;

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
            if (drillTip.IsDrilling == false)
            {
                return;
            }

            drillDistance = Mathf.Max(drillDistance, Vector3.Distance(drillStartPosition, drillTip.transform.position));
        }

        private void OnDrawGizmos()
        {
            if (drillTip == null || drillTip.IsDrilling == false)
            {
                return;
            }

            Vector3 drillEndPosition = drillStartPosition + drillDirection * drillDistance;
            DebugUtils.DrawCylinderGizmo(drillStartPosition, drillEndPosition, holeWidth, Color.red);
        }
    }
}