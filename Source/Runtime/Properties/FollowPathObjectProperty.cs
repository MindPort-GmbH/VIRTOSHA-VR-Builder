/*
<ai_context>
  FollowPathObjectProperty requires an interactable property and exposes an IFollowPathObjectTip.
</ai_context>
*/

using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.BasicInteraction.Properties; // Example usage if relevant for "interactable"
using VRBuilder.VIRTOSHA.Properties;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Concrete implementation of a property referencing an interactable object and a tip for path following.
    /// </summary>
    [RequireComponent(typeof(IUsableProperty))] // or any other relevant interaction property
    public class FollowPathObjectProperty : LockableProperty, IFollowPathObjectProperty
    {
        [SerializeField]
        [Tooltip("Reference to a component that implements IFollowPathObjectTip.")]
        private MonoBehaviour tipReference;

        /// <summary>
        /// The tip that will be used to measure path deviation.
        /// </summary>
        public IFollowPathObjectTip FollowPathTip
        {
            get
            {
                return tipReference as IFollowPathObjectTip;
            }
        }

        protected override void InternalSetLocked(bool lockState)
        {
            // Optionally restrict usage if locked. For example:
            // var usableProp = GetComponent<IUsableProperty>();
            // if (usableProp != null) { usableProp.SetLocked(lockState); }
        }
    }
}