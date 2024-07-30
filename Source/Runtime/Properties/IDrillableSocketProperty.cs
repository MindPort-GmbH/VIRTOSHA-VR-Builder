using UnityEngine;
using VRBuilder.Core.Properties;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property for a scene object that represents a drillable hole.
    /// </summary>
    public interface IDrillableSocketProperty : ISceneObjectProperty
    {
        public float Width { get; }
        public Vector3 EnterPoint { get; }
        public Vector3 EndPoint { get; }
        public float EnterTolerance { get; }
        public float EndTolerance { get; }
        public float WidthTolerance { get; }
    }
}