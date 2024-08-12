using UnityEngine;

namespace VRBuilder.VIRTOSHA.Structs
{
    /// <summary>
    /// Defines a hole drilled on a game object.
    /// </summary>
    public struct Hole
    {
        /// <summary>
        /// The start point of the hole in local space.
        /// </summary>
        public Vector3 EnterPoint { get; private set; }

        /// <summary>
        /// The end point of the hole in local space.
        /// </summary>
        public Vector3 EndPoint { get; private set; }

        /// <summary>
        /// The width of the hole.
        /// </summary>
        public float Width { get; private set; }

        public Hole(Vector3 enterPoint, Vector3 endPoint, float width)
        {
            EnterPoint = enterPoint;
            EndPoint = endPoint;
            Width = width;
        }

        public override string ToString()
        {
            return $"[{EnterPoint}-{EndPoint}W{Width}]";
        }
    }
}