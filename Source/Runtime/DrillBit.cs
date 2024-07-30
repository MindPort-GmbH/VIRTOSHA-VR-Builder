using System;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA
{
    /// <summary>
    /// Defines a drill bit used by a <see cref="Drill"/> to drill holes.
    /// </summary>
    public class DrillBit : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Width of the holes created by this drill bit.")]
        private float width = 0.05f;

        [SerializeField]
        [Tooltip("Tip associated with this drill bit. If null, it will attempt to find one in its children.")]
        private DrillTip drillTip;

        /// <summary>
        /// Called when the tip of the drill bit touches an object which is drillable.
        /// </summary>
        public event EventHandler<DrillBitEventArgs> TouchedDrillableObject;

        /// <summary>
        /// True while the drill is being used.
        /// </summary>
        public bool InUse { get; set; }

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