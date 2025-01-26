using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.VIRTOSHA.Components;
using VRBuilder.XRInteraction.Properties;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Concrete implementation of a property referencing an interactable object and a tip for path following.
    /// </summary>
    public class FollowPathObjectProperty : GrabbableProperty, IFollowPathObjectProperty
    {
        [SerializeField]
        private FollowPathObjectTip followPathObjectTip;

        /// <summary>
        /// The tip that will be used to measure path deviation.
        /// </summary>
        public IFollowPathObjectTip FollowPathTip => followPathObjectTip;

        private void OnValidate()
        {
            if (followPathObjectTip == null)
            {
                followPathObjectTip = GetComponentInChildren<FollowPathObjectTip>();
                if (followPathObjectTip == null)
                {
                    Debug.LogError($"{nameof(FollowPathObjectProperty)} on '{gameObject.name}' is missing a FollowPathObjectTip component in its children.");
                }
            }
        }
    }
}