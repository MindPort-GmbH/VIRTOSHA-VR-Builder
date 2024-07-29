using System;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    public class DrillBit : MonoBehaviour
    {
        [SerializeField]
        private float width = 0.05f;

        [SerializeField]
        private DrillTip drillTip;

        public event EventHandler<DrillBitEventArgs> TouchedDrillableObject;
        public bool IsUsing { get; set; }

        private void Awake()
        {
            if (drillTip == null)
            {
                drillTip = GetComponentInChildren<DrillTip>();
            }

            drillTip.Init(this);
        }

        public Transform Tip
        {
            get
            {
                if (drillTip == null)
                {
                    drillTip = GetComponentInChildren<DrillTip>();
                }
                return drillTip.transform;
            }
        }

        public Transform Base => transform;
        public float Width => width;

        public void EmitTouchedDrillableObject(IDrillableProperty drillableProperty, Collider otherCollider)
        {
            TouchedDrillableObject?.Invoke(this, new DrillBitEventArgs(drillableProperty, otherCollider));
        }
    }

    public class DrillBitEventArgs : EventArgs
    {
        public readonly IDrillableProperty DrillableProperty;
        public readonly Collider OtherCollider;

        public DrillBitEventArgs(IDrillableProperty drillableProperty, Collider otherCollider)
        {
            DrillableProperty = drillableProperty;
            OtherCollider = otherCollider;
        }
    }
}