/*
<ai_context>
  This interface exposes an object with an IFollowPathObjectTip to measure path following.
</ai_context>
*/

using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property that references an interactable object and a tip used to follow a spline path.
    /// </summary>
    public interface IFollowPathObjectProperty : ISceneObjectProperty, ILockable
    {
        /// <summary>
        /// The tip that will follow the path.
        /// </summary>
        IFollowPathObjectTip FollowPathTip { get; }
    }
}