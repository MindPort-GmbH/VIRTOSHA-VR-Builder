using System;
using System.Linq;
using UnityEngine;
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Components
{
    [RequireComponent(typeof(Collider))]
    public class FollowPathObjectTip : MonoBehaviour, IFollowPathObjectTip
    {
        /// <summary>
        /// Called when the tip of the object bit touches an object which is cuttable.
        /// </summary>
        public event EventHandler<FollowPathObjectEventArgs> TouchedCuttableObject;

        public Transform TipTransform { get; }

        private Collider tipCollider;

        private void Awake()
        {
            tipCollider = GetComponent<Collider>();
            if (tipCollider == null)
            {
                Debug.LogError($"No collider is found on {typeof(FollowPathObjectTip).Name} or its children on '{gameObject.name}'. A collider is needed for the component to work as intended. Please add one.");
            }
            else if (tipCollider.isTrigger == false)
            {
                Debug.LogError($"The collider on {typeof(FollowPathObjectTip).Name} or its children on '{gameObject.name}' is not set as a trigger. Please set 'Is Trigger' to true in the Inspector for the component to work as intended.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IFollowPathProperty followPathProperty = other.GetComponentInParent<IFollowPathProperty>();

            if (followPathProperty != null && followPathProperty.Colliders.Contains(other) && followPathProperty.IsLocked == false)
            {
                TouchedCuttableObject?.Invoke(this, new FollowPathObjectEventArgs(this, other));
            }
        }

        public class FollowPathObjectEventArgs : EventArgs
        {
            public readonly IFollowPathObjectTip FollowPathObjectTip;
            public readonly Collider OtherCollider;

            public FollowPathObjectEventArgs(IFollowPathObjectTip followPathObjectTip, Collider otherCollider)
            {
                FollowPathObjectTip = followPathObjectTip;
                OtherCollider = otherCollider;
            }
        }
    }
}
