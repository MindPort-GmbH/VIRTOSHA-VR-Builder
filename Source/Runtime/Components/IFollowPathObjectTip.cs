/*
<ai_context>
  This interface describes the tip used to measure deviation for following a path.
</ai_context>
*/

using System;
using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
  /// <summary>
  /// Represents the tip object used for measuring deviations along a pat  /  ///  
  /// </summary>
  public interface IFollowPathObjectTip
  {
    /// <summary>
    /// The transform used to measure the path-following position/angles.
    /// </summary>
    public Transform TipTransform { get; }

    /// <summary>
    /// Occurs when the tip touches the collider of a object that is follow able.
    /// </summary>
    public event EventHandler<FollowPathObjectTip.FollowPathObjectEventArgs> TouchedCuttableObject;
  }
}