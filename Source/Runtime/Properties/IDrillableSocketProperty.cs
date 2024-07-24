using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property for a scene object that represents a drillable hole.
    /// </summary>
    public interface IDrillableSocketProperty : ISceneObjectProperty, ILockable
    {
        void SetHoleProperties(float depth, float width);

        float DrilledDepth { get; }

        float DrilledWidth { get; }
    }
}