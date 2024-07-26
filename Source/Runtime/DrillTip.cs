using System;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    public class DrillTip : MonoBehaviour
    {
        public event EventHandler<DrillTipEventArgs> TouchedDrillableObject;
        public bool IsUsing { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (IsUsing == false)
            {
                return;
            }

            IDrillableProperty drillableProperty = other.GetComponent<IDrillableProperty>();

            if (drillableProperty != null)
            {
                TouchedDrillableObject?.Invoke(this, new DrillTipEventArgs(drillableProperty, other));
            }
        }
    }

    public class DrillTipEventArgs : EventArgs
    {
        public readonly IDrillableProperty DrillableProperty;
        public readonly Collider OtherCollider;

        public DrillTipEventArgs(IDrillableProperty drillableProperty, Collider otherCollider)
        {
            DrillableProperty = drillableProperty;
            OtherCollider = otherCollider;
        }
    }
}