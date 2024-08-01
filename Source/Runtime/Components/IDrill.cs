using System;
using UnityEngine;

namespace VRBuilder.VIRTOSHA.Components
{
    public interface IDrill
    {
        event EventHandler<DrillEventArgs> DrillingStarted;
        event EventHandler<DrillEventArgs> DrillingStopped;

        bool IsDrilling { get; }
        float CurrentDrillDepth { get; }
    }

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