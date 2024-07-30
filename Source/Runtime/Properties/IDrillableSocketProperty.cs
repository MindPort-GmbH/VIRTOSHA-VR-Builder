using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property for a scene object that represents a drillable hole.
    /// </summary>
    public interface IDrillableSocketProperty : ISceneObjectProperty
    {
        /// <summary>
        /// Expected width of the drillable hole.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// Expected enter point of the hole.
        /// </summary>
        public Vector3 EnterPoint { get; }

        /// <summary>
        /// Expected end point of the hole.
        /// </summary>
        public Vector3 EndPoint { get; }

        /// <summary>
        /// Distance from the expected enter point which will still be considered valid.
        /// </summary>
        public float EnterTolerance { get; }

        /// <summary>
        /// Distance from the expected end point which will still be considered valid.
        /// </summary>
        public float EndTolerance { get; }

        /// <summary>
        /// Acceptable difference in width with the actual hole.
        /// </summary>
        public float WidthTolerance { get; }
    }
}