using UnityEngine;
using VRBuilder.Core.Properties;
using VRBuilder.Core.SceneObjects;

namespace VRBuilder.VIRTOSHA.Properties
{
    /// <summary>
    /// Property for an object that can be drilled holes into.
    /// </summary>
    public interface IDrillableProperty : ISceneObjectProperty, ILockable
    {
        void CreateHole(Vector3 startPosition, Vector3 endPosition, float width);

        bool HasHole(Vector3 startPosition, Vector3 endPosition, float width, float startTolerance, float endTolerance, float widthTolerance);
    }
}