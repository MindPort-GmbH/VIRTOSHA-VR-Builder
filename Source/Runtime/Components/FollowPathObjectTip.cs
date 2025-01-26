using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Component that implements IFollowPathObjectTip to provide Transform 
    /// for measuring path-following.
    /// </summary>
    public class FollowPathObjectTip : MonoBehaviour, IFollowPathObjectTip
    {
        /// <inheritdoc />
        public Transform TipTransform
        {
            get
            {
                return transform;
            }
        }
    }
}
