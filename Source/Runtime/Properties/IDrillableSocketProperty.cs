using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.VIRTOSHA.Structs;

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
        float Width { get; }

        /// <summary>
        /// Expected enter point of the hole.
        /// </summary>
        Vector3 EnterPoint { get; }

        /// <summary>
        /// Expected end point of the hole.
        /// </summary>
        Vector3 EndPoint { get; }

        /// <summary>
        /// Distance from the expected enter point which will still be considered valid.
        /// </summary>
        float EnterTolerance { get; }

        /// <summary>
        /// Distance from the expected end point which will still be considered valid.
        /// </summary>
        float EndTolerance { get; }

        /// <summary>
        /// Acceptable difference in width with the actual hole.
        /// </summary>
        float WidthTolerance { get; }

        /// <summary>
        /// Configures the drillable socket with the desired values.
        /// </summary>
        void Configure(Hole hole, float enterTolerance = 0.01f, float endTolerance = 0.01f, float widthTolerance = 0.001f, bool placeEnterPointOnSurface = true);
    }
}