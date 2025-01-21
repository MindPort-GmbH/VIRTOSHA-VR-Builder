using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;
using VRBuilder.VIRTOSHA.Components;

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