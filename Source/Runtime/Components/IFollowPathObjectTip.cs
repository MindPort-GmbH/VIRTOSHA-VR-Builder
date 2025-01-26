using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
  /// <summary>
  /// Represents the tip object used for measuring deviations along a path.
  /// </summary>
  public interface IFollowPathObjectTip
  {
    /// <summary>
    /// The Transform of the tip object, used to measure the path-following position and rotation.
    /// </summary>
    Transform TipTransform { get; }
  }
}