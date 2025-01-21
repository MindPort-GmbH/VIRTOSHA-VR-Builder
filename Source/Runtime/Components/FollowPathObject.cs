/*
<ai_context>
  FollowPathObjectProperty requires an interactable property and exposes an IFollowPathObjectTip.
</ai_context>
*/

using System;
using UnityEngine;
using VRBuilder.BasicInteraction.Properties;
using VRBuilder.VIRTOSHA.Properties;
using VRBuilder.XRInteraction.Properties;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Concrete implementation of a property referencing an interactable object and a tip for path following.
    /// </summary>
    [RequireComponent(typeof(GrabbableProperty))]
    public class FollowPathObjectProperty : MonoBehaviour, IFollowPathObject
    {
        [SerializeField]
        [Tooltip("Reference to a component that implements IFollowPathObjectTip.")]
        private IFollowPathObjectTip followPathTip;

        /// <summary>
        /// The tip that will be used to measure path deviation.
        /// </summary>
        public IFollowPathObjectTip FollowPathTip { get; }

        public IGrabbableProperty grabbableProperty;

        private void OnEnable()
        {
            if (followPathTip == null)
            {
                followPathTip = GetComponentInChildren<IFollowPathObjectTip>();
            }

            grabbableProperty = GetComponent<IGrabbableProperty>();
            grabbableProperty.GrabStarted.AddListener(OnGrabStarted);
            grabbableProperty.GrabEnded.AddListener(OnGrabEnded);
        }

        private void OnDisable()
        {
            if (grabbableProperty != null)
            {
                grabbableProperty.GrabStarted.RemoveListener(OnGrabStarted);
            }
            if (grabbableProperty != null)
            {
                grabbableProperty.GrabEnded.RemoveListener(OnGrabEnded);
            }
        }

        private void OnGrabEnded(GrabbablePropertyEventArgs arg0)
        {
            throw new NotImplementedException();
        }

        private void OnGrabStarted(GrabbablePropertyEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}