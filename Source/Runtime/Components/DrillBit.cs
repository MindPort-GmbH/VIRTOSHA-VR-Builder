using System;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Defines a drill bit used by a <see cref="Drill"/> to drill holes.
    /// Needs a reference to a <see cref="DrillTip"/> component.
    /// </summary>
    public class DrillBit : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Width of the holes created by this drill bit.")]
        private float width = 0.05f;

        [SerializeField]
        [Tooltip("Tip associated with this drill bit. If null, it will attempt to find one first in the GameObject then its children.)]
        private DrillTip drillTip;

        /// <summary>
        /// Called when the tip of the drill bit touches an object which is drillable.
        /// </summary>
        public event EventHandler<DrillBitEventArgs> TouchedDrillableObject;

        /// <summary>
        /// True while the drill is being used.
        /// </summary>
        public bool InUse { get; set; }

        /// <summary>
        /// Returns the base of the drill bit.
        /// </summary>
        public Transform Base => transform;

        /// <summary>
        /// Width of the holes created by this drill bit.
        /// </summary>
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        private void Awake()
        {
            CheckForDrillTip();
            drillTip.Init(this);
        }

        /// <summary>
        /// Gets the transform of the drill tip.
        /// </summary>
        /// <returns>The transform of the drill tip.</returns>
        public Transform Tip
        {
            get
            {
                CheckForDrillTip();
                return drillTip.transform;
            }
        }

        /// <summary>
        /// Emits an event when a drillable object is touched by the drill bit.
        /// </summary>
        /// <param name="drillableProperty">The drillable property of the object being touched.</param>
        /// <param name="otherCollider">The collider of the object being touched.</param>
        public void EmitTouchedDrillableObject(IDrillableProperty drillableProperty, Collider otherCollider)
        {
            TouchedDrillableObject?.Invoke(this, new DrillBitEventArgs(drillableProperty, otherCollider));
        }

        private void CheckForDrillTip()
        {
            if (drillTip == null)
            {
                drillTip = GetComponentInChildren<DrillTip>();
            }

            if (drillTip == null)
            {
                throw new NullReferenceException($"The {typeof(DrillBit).Name} component on '{gameObject.name}' cannot work without a {typeof(DrillTip).Name} component on a child object. Please create one.");
            }
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