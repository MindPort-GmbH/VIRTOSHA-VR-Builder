using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
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
        Vector3 drillEndPosition;

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
        }

        private void OnUseEnded(UsablePropertyEventArgs args)
        {
            drillTip.IsDrilling = false;
            drillTip.TouchedDrillableObject -= OnTouchedDrillableObject;

            if (currentDrilledObject != null)
            {
                // Get final position

                // Create hole in object

                currentDrilledObject = null;
            }
        }
    }
}