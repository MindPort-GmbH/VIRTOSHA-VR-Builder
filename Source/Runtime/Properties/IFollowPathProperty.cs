/*
<ai_context>
  This interface defines a property for following a spline path with various deviation constraints and events.
</ai_context>
*/

using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.VIRTOSHA.Properties
{
  /// <summary>
  /// Interface for a property that manages following a splin  /// path, storing progress, events, and deviation data.
  /// </summary>
  public interface IFollowPathProperty : ISceneObjectProperty, ILockable
  {
    /// <summary>
    /// True if the path has been fully followed.
    /// </summary>
    bool IsPathCompleted { get; }

    /// <summary>
    /// The object that follows the path.
    /// </summary>
    IFollowPathObjectProperty CurrentFollowPathObjectProperty { get; set; }

    /// <summary>
    /// Force-complete the path (e.g., for fast-forwarding).
    /// </summary>
    void CompletePath();
  }
}