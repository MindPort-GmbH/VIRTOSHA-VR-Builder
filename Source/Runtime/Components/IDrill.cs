using System;
using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    /// <summary>
    /// An object that can drill holes in a drillable property.
    /// </summary>
    public interface IDrill
    {
        /// <summary>
        /// Called when the object starts drilling a hole.
        /// </summary>
        event EventHandler<DrillEventArgs> DrillingStarted;

        /// <summary>
        /// Called when the object stops drilling the hole.
        /// </summary>
        event EventHandler<DrillEventArgs> DrillingStopped;

        /// <summary>
        /// True while the object is drilling.
        /// </summary>
        bool IsDrilling { get; }

        /// <summary>
        /// Current depth of the hole.
        /// </summary>
        float CurrentDrillDepth { get; }
    }

    /// <summary>
    /// Event args for <see cref="IDrill"/> events.
    /// </summary>
    public class DrillEventArgs : EventArgs
    {
        public readonly Vector3 EnterPoint;
        public readonly Vector3 Direction;
        public readonly float HoleWidth;

        public DrillEventArgs(Vector3 enterPoint, Vector3 direction, float holeWidth)
        {
            EnterPoint = enterPoint;
            Direction = direction;
            HoleWidth = holeWidth;
        }
    }
}