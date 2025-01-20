/*
<ai_context>
  This interface describes the tip used to measure deviation for following a path.
</ai_context>
*/

using UnityEngine;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Represents the tip object used for measuring deviations along a path.
    /// </summary>
    public interface IFollowPathObjectTip
    {
        /// <summary>
        /// The transform used to measure the path-following position/angles.
        /// </summary>
        Transform TipTransform { get; }
    }
}