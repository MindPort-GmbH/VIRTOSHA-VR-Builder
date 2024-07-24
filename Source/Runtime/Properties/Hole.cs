using UnityEngine;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Defines a hole drilled on a game object.
    /// </summary>
    public struct Hole
    {
        /// <summary>
        /// The start point of the hole in local space.
        /// </summary>
        public Vector3 Start { get; private set; }

        /// <summary>
        /// The end point of the hole in local space.
        /// </summary>
        public Vector3 End { get; private set; }

        /// <summary>
        /// The width of the hole.
        /// </summary>
        public float Width { get; private set; }

        public Hole(Vector3 start, Vector3 end, float width)
        {
            Start = start;
            End = end;
            Width = width;
        }
    }
}