using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// Component that implements IFollowPathObjectTip to provide position and rotation vectors 
    /// for measuring path-following.
    /// </summary>
    public class FollowPathObjectTip : MonoBehaviour, IFollowPathObjectTip
    {
        /// <summary>
        /// The vector used to measure the path-following position.
        /// </summary>
        public Vector3 TipPosition
        {
            get
            {
                return transform.position;
            }
        }

        /// <summary>
        /// The vector used to measure the path-following angles.
        /// </summary>
        public Vector3 TipRotation
        {
            get
            {
                return transform.eulerAngles;
            }
        }
    }
}
