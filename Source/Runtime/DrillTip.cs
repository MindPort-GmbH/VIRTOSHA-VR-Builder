using System;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    public class DrillTip : MonoBehaviour
    {
        public event EventHandler<DrillTipEventArgs> TouchedDrillableObject;
        public bool IsDrilling { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (IsDrilling == false)
            {
                return;
            }

            IDrillableProperty drillableProperty = other.GetComponent<IDrillableProperty>();

            if (drillableProperty != null)
            {
                TouchedDrillableObject?.Invoke(this, new DrillTipEventArgs(drillableProperty));
            }
        }
    }

    public class DrillTipEventArgs : EventArgs
    {
        public readonly IDrillableProperty DrillableProperty;

        public DrillTipEventArgs(IDrillableProperty drillableProperty)
        {
            DrillableProperty = drillableProperty;
        }
    }
}