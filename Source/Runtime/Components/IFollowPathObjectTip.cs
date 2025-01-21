using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
  /// <summary>
  /// Represents the tip object used for measuring deviations along a path.
  /// </summary>
  public interface IFollowPathObjectTip
  {
    /// <summary>
    /// The vector used to measure the path-following position.
    /// </summary>
    Vector3 TipPosition { get; }
    /// <summary>
    /// The vector used to measure the path-following angles.
    /// </summary>
    Vector3 TipRotation { get; }
  }
}